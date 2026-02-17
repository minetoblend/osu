// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Timing;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCard
    {
        public partial class SongPreviewContainer : Container, IBeatSyncProvider
        {
            [Resolved]
            private PreviewTrackManager previewTrackManager { get; set; } = null!;

            protected override Container<Drawable> Content { get; }

            public SongPreviewContainer()
            {
                InternalChildren =
                [
                    new PulsingContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children =
                        [
                            Content = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                        ]
                    },
                ];
            }

            private PreviewTrack? previewTrack;

            public void LoadPreview(APIBeatmap beatmap)
            {
                Debug.Assert(previewTrack == null);

                LoadComponentAsync(previewTrack = previewTrackManager.Get(beatmap.BeatmapSet!), track =>
                {
                    AddInternal(track);

                    setupBeatSyncProvider(track, beatmap);

                    if (IsHovered)
                        startPreviewIfAvailable();
                });
            }

            protected override bool OnHover(HoverEvent e)
            {
                startPreviewIfAvailable();

                return base.OnHover(e);
            }

            private void startPreviewIfAvailable() => previewTrack?.Start();

            private partial class PulsingContainer : BeatSyncedContainer
            {
                protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
                {
                    if (!IsBeatSyncedWithTrack)
                        return;

                    this.ScaleTo(1.02f, timingPoint.BeatLength * 0.2f)
                        .Then()
                        .ScaleTo(1f, timingPoint.BeatLength, new CubicBezierEasingFunction(easeIn: 0.1f, easeOut: 1f));
                }
            }

            #region IBeatSyncProvider implementation

            private readonly PreviewTrackClock beatSyncClock = new PreviewTrackClock();
            private readonly ControlPointInfo controlPoints = new ControlPointInfo();

            ChannelAmplitudes IHasAmplitudes.CurrentAmplitudes => ChannelAmplitudes.Empty;
            ControlPointInfo IBeatSyncProvider.ControlPoints => controlPoints;
            IClock IBeatSyncProvider.Clock => beatSyncClock;

            private void setupBeatSyncProvider(PreviewTrack track, APIBeatmap beatmap)
            {
                beatSyncClock.Track = track;

                controlPoints.Add(0, new TimingControlPoint
                {
                    BeatLength = beatmap.BPM > 0 ? 60_000 / beatmap.BPM : TimingControlPoint.DEFAULT_BEAT_LENGTH
                });
            }

            private class PreviewTrackClock : IClock
            {
                public PreviewTrack? Track { get; set; }

                public double CurrentTime => Track?.CurrentTime ?? 0;
                public double Rate => 1;
                public bool IsRunning => Track?.IsRunning ?? false;
            }

            #endregion
        }
    }
}
