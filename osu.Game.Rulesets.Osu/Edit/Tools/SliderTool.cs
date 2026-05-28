// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit.Tools
{
    public class SliderTool : OsuHitObjectPlacementTool<Slider>
    {
        public SliderTool()
            : base(new Slider())
        {
            AddInternal(new SliderPathVisualizer(HitObject));
        }

        private readonly List<PathControlPoint> path = [new PathControlPoint(Vector2.Zero, PathType.BEZIER)];

        [Resolved]
        private BindableBeatDivisor beatDivisor { get; set; } = null!;

        protected override void UpdateTimeAndPosition(Vector2 position, double time)
        {
            switch (State)
            {
                case PlacementState.Idle:
                    var snapResult = SnapProvider.SnapToHitObjects(position, exclude: [HitObject]);

                    if (snapResult != null)
                        position = snapResult.Position;

                    HitObject.StartTime = EditorClock.CurrentTime;
                    HitObject.Position = position;
                    break;

                case PlacementState.Active:
                    position -= HitObject.StackedPosition;

                    HitObject.Path.ControlPoints.Clear();
                    HitObject.Path.ControlPoints.AddRange(path);

                    if (Vector2.Distance(path[^1].Position, position) > 3)
                        HitObject.Path.ControlPoints.Add(new PathControlPoint(position));

                    resnapSliderLength();

                    if (Precision.DefinitelyBigger(HitObject.SpanDuration, 0))
                        HitObject.RepeatCount = int.Clamp((int)((time - HitObject.StartTime) / HitObject.SpanDuration), 0, 100);

                    break;
            }
        }

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button == MouseButton.Right)
            {
                EndPlacement(State == PlacementState.Active);

                return true;
            }

            return base.OnMouseDown(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            switch (State)
            {
                case PlacementState.Idle:
                    BeginPlacement();
                    break;

                case PlacementState.Active:
                    var position = playfield.ScreenSpaceToGamefield(e.ScreenSpaceMousePosition) - HitObject.StackedPosition;

                    if (Vector2.Distance(position, path[^1].Position) < 3)
                    {
                        path[^1].Type = PathType.BEZIER;
                    }
                    else
                    {
                        path.Add(new PathControlPoint(position));
                    }

                    break;
            }

            return true;
        }

        [Resolved]
        private IBeatSnapProvider beatSnapProvider { get; set; } = null!;

        private void resnapSliderLength()
        {
            double snapReferenceTime = HitObject.StartTime;

            double actualDuration = snapReferenceTime + HitObject.Path.CalculatedDistance / HitObject.Velocity;

            double snappedTime = beatSnapProvider.SnapTime(actualDuration, snapReferenceTime);

            double beatLength = beatSnapProvider.GetBeatLengthAtTime(snapReferenceTime);

            // we don't want to exceed the actual duration and snap to a point in the future.
            // as we are snapping to beat length via SnapTime (which will round-to-nearest), check for snapping in the forward direction and reverse it.
            if (snappedTime > actualDuration + 1)
                snappedTime -= beatLength;

            HitObject.Path.ExpectedDistance.Value = Math.Min((snappedTime - snapReferenceTime) * HitObject.Velocity, HitObject.Path.CalculatedDistance);
        }
    }
}
