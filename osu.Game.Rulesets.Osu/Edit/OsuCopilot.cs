// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using System.Linq;
using System.Text;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Beatmaps.Timing;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.IO;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class OsuCopilot : CompositeDrawable
    {
        public readonly Bindable<bool> RequestActive = new Bindable<bool>();

        [Resolved]
        private OsuHitObjectComposer composer { get; set; } = null!;

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        private readonly Bindable<CompositionTool> activeTool = new Bindable<CompositionTool>();

        private readonly BindableBool isActive = new BindableBool();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activeTool.BindValueChanged(toolChanged, true);
        }

        protected override void Update()
        {
            base.Update();

            activeTool.Value = composer.BlueprintContainer.CurrentTool;
        }

        [Resolved]
        private Storage storage { get; set; }

        [Resolved]
        private IAPIProvider api { get; set; }

        [Resolved]
        private EditorClock clock { get; set; }

        private void toolChanged(ValueChangedEvent<CompositionTool> tool)
        {
            isActive.Value = !(tool.NewValue is SelectTool || tool.NewValue is GridFromPointsTool);
            if (!isActive.Value)
                return;
        }

        private CopilotRequest? request;

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.Tab)
            {
                return SendRequest();
            }

            return false;
        }

        public bool SendRequest()
        {
            if (request != null)
                return false;

            string? fileStorePath = beatmap.BeatmapInfo.BeatmapSet?.GetPathForFile(beatmap.Metadata.AudioFile);

            if (fileStorePath == null)
                return false;

            string? fullPath = storage.GetFullPath($"files/{fileStorePath}");

            if (fullPath == null)
                return false;

            var writer = new StringWriter();

            int oldVersion = beatmap.BeatmapVersion;
            beatmap.BeatmapVersion = 14;

            int startTime = (int)clock.CurrentTime;
            int endTime = startTime + 3000;

            bool isInRange(HitObject h) => h.StartTime >= startTime && h.StartTime <= endTime;

            MutateBeatmap(beatmap.BeatmapInfo.BeatmapSet!, beatmap.PlayableBeatmap);

            var removed = beatmap.HitObjects.Where(isInRange).ToList();
            beatmap.RemoveRange(removed);

            new LegacyBeatmapEncoder(beatmap, beatmap.BeatmapSkin).Encode(writer);

            if (removed.Count > 0)
                Schedule(() => beatmap.AddRange(removed));

            beatmap.BeatmapVersion = oldVersion;

            request = new CopilotRequest
            {
                BeatmapString = writer.ToString(),
                StartTime = startTime,
                EndTime = endTime,
                AudioPath = fullPath
            };

            request.Success += result => Schedule(() =>
            {
                handleResult(request, result);
                RequestActive.Value = false;
                request = null;
            });
            request.Failure += exception =>
            {
                Logger.Error(exception, "Error sending request");
                RequestActive.Value = false;
                RequestActive.Value = false;
                request = null;
            };

            api.Queue(request);

            RequestActive.Value = true;

            return true;
        }

        private void handleResult(CopilotRequest request, CopilotResult result)
        {
            if (!result.Success || result.Result == null)
                return;

            Beatmap beatmap;
            using (var s = new MemoryStream(Encoding.UTF8.GetBytes(result.Result)))
                beatmap = new LegacyBeatmapDecoder().Decode(new LineBufferedReader(s));

            var osuBeatmap = new OsuBeatmapConverter(beatmap, new OsuRuleset()).Convert();

            bool isInRange(HitObject h) => h.StartTime >= request.StartTime && h.StartTime <= request.EndTime;

            var hitObjects = osuBeatmap.HitObjects.Where(isInRange);

            this.beatmap.RemoveRange(this.beatmap.HitObjects.Where(isInRange).ToList());

            this.beatmap.AddRange(hitObjects);
        }

        protected virtual void MutateBeatmap(BeatmapSetInfo beatmapSet, IBeatmap playableBeatmap)
        {
            // Convert beatmap elements to be compatible with legacy format
            // So we truncate time and position values to integers, and convert paths with multiple segments to Bézier curves

            // We must first truncate all timing points and move all objects in the timing section with it to ensure everything stays snapped
            for (int i = 0; i < playableBeatmap.ControlPointInfo.TimingPoints.Count; i++)
            {
                var timingPoint = playableBeatmap.ControlPointInfo.TimingPoints[i];
                double offset = Math.Floor(timingPoint.Time) - timingPoint.Time;
                double nextTimingPointTime = i + 1 < playableBeatmap.ControlPointInfo.TimingPoints.Count
                    ? playableBeatmap.ControlPointInfo.TimingPoints[i + 1].Time
                    : double.PositiveInfinity;

                // Offset all control points in the timing section (including the current one)
                foreach (var controlPoint in playableBeatmap.ControlPointInfo.AllControlPoints.Where(o => o.Time >= timingPoint.Time && o.Time < nextTimingPointTime))
                    controlPoint.Time += offset;

                // Offset all hit objects in the timing section
                foreach (var hitObject in playableBeatmap.HitObjects.Where(o => o.StartTime >= timingPoint.Time && o.StartTime < nextTimingPointTime))
                    hitObject.StartTime += offset;
            }

            foreach (var controlPoint in playableBeatmap.ControlPointInfo.AllControlPoints)
                controlPoint.Time = Math.Floor(controlPoint.Time);

            for (int i = 0; i < playableBeatmap.Breaks.Count; i++)
                playableBeatmap.Breaks[i] = new BreakPeriod(Math.Floor(playableBeatmap.Breaks[i].StartTime), Math.Floor(playableBeatmap.Breaks[i].EndTime));

            foreach (var hitObject in playableBeatmap.HitObjects)
            {
                // Truncate end time before truncating start time because end time is dependent on start time
                if (hitObject is IHasDuration hasDuration && hitObject is not IHasPath)
                    hasDuration.Duration = Math.Floor(hasDuration.EndTime) - Math.Floor(hitObject.StartTime);

                hitObject.StartTime = Math.Floor(hitObject.StartTime);

                if (hitObject is IHasXPosition hasXPosition)
                    hasXPosition.X = MathF.Round(hasXPosition.X);

                if (hitObject is IHasYPosition hasYPosition)
                    hasYPosition.Y = MathF.Round(hasYPosition.Y);

                if (hitObject is not IHasPath hasPath) continue;

                // stable's hit object parsing expects the entire slider to use only one type of curve,
                // and happens to use the last non-empty curve type read for the entire slider.
                // this clear of the last control point type handles an edge case
                // wherein the last control point of an otherwise-single-segment slider path has a different type than previous,
                // which would lead to sliders being mangled when exported back to stable.
                // normally, that would be handled by the `BezierConverter.ConvertToModernBezier()` call below,
                // which outputs a slider path containing only BEZIER control points,
                // but a non-inherited last control point is (rightly) not considered to be starting a new segment,
                // therefore it would fail to clear the `CountSegments() <= 1` check.
                // by clearing explicitly we both fix the issue and avoid unnecessary conversions to BEZIER.
                if (hasPath.Path.ControlPoints.Count > 1)
                    hasPath.Path.ControlPoints[^1].Type = null;

                if (BezierConverter.CountSegments(hasPath.Path.ControlPoints) <= 1
                    && hasPath.Path.ControlPoints[0].Type!.Value.Degree == null) continue;

                var convertedToBezier = BezierConverter.ConvertToModernBezier(hasPath.Path.ControlPoints);

                hasPath.Path.ControlPoints.Clear();

                for (int i = 0; i < convertedToBezier.Count; i++)
                {
                    var convertedPoint = convertedToBezier[i];

                    // Truncate control points to integer positions
                    var position = new Vector2(
                        (float)Math.Floor(convertedPoint.Position.X),
                        (float)Math.Floor(convertedPoint.Position.Y));

                    // stable only supports a single curve type specification per slider.
                    // we exploit the fact that the converted-to-Bézier path only has Bézier segments,
                    // and thus we specify the Bézier curve type once ever at the start of the slider.
                    hasPath.Path.ControlPoints.Add(new PathControlPoint(position, i == 0 ? PathType.BEZIER : null));

                    // however, the Bézier path as output by the converter has multiple segments.
                    // `LegacyBeatmapEncoder` will attempt to encode this by emitting per-control-point curve type specs which don't do anything for stable.
                    // instead, stable expects control points that start a segment to be present in the path twice in succession.
                    if (convertedPoint.Type == PathType.BEZIER && i > 0)
                        hasPath.Path.ControlPoints.Add(new PathControlPoint(position));
                }
            }
        }
    }
}
