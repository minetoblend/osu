// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Overlays;

namespace osu.Game.Screens.Edit.Compose.Components.Timeline
{
    public partial class DefaultTimeline : Timeline
    {
        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        private const float timeline_height = 80;

        private readonly Drawable userContent;

        private bool alwaysShowControlPoints;

        public bool AlwaysShowControlPoints
        {
            get => alwaysShowControlPoints;
            set
            {
                if (value == alwaysShowControlPoints)
                    return;

                alwaysShowControlPoints = value;
                controlPointsVisible.TriggerChange();
            }
        }

        private WaveformGraph waveform = null!;

        private TimelineTickDisplay ticks = null!;
        private Bindable<bool> ticksVisible = null!;

        private Bindable<float> waveformOpacity = null!;
        private Bindable<bool> controlPointsVisible = null!;

        private TimelineTimingChangeDisplay controlPoints = null!;

        private readonly IBindable<Track> track = new Bindable<Track>();

        public DefaultTimeline(Drawable userContent)
            : base()
        {
            this.userContent = userContent;

            Height = timeline_height;
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap, OsuColour colours, OverlayColourProvider colourProvider, OsuConfigManager config)
        {
            CentreMarker centreMarker;

            // We don't want the centre marker to scroll
            AddInternal(centreMarker = new CentreMarker());

            AddRange(new Drawable[]
            {
                ticks = new TimelineTickDisplay(),
                new Box
                {
                    Name = "zero marker",
                    RelativeSizeAxes = Axes.Y,
                    Width = TimelineTickDisplay.TICK_WIDTH / 2,
                    Origin = Anchor.TopCentre,
                    Colour = colourProvider.Background1,
                },
                controlPoints = new TimelineTimingChangeDisplay
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = timeline_height,
                    Children = new[]
                    {
                        waveform = new WaveformGraph
                        {
                            RelativeSizeAxes = Axes.Both,
                            BaseColour = colours.Blue.Opacity(0.2f),
                            LowColour = colours.BlueLighter,
                            MidColour = colours.BlueDark,
                            HighColour = colours.BlueDarker,
                        },
                        centreMarker.CreateProxy(),
                        ticks.CreateProxy(),
                        userContent,
                    }
                },
            });

            waveformOpacity = config.GetBindable<float>(OsuSetting.EditorWaveformOpacity);
            controlPointsVisible = config.GetBindable<bool>(OsuSetting.EditorTimelineShowTimingChanges);
            ticksVisible = config.GetBindable<bool>(OsuSetting.EditorTimelineShowTicks);

            track.BindTo(editorClock.Track);
            track.BindValueChanged(_ =>
            {
                waveform.Waveform = beatmap.Value.Waveform;
                Scheduler.AddOnce(applyVisualOffset, beatmap);
            }, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            waveformOpacity.BindValueChanged(_ => updateWaveformOpacity(), true);

            ticksVisible.BindValueChanged(visible => ticks.FadeTo(visible.NewValue ? 1 : 0, 200, Easing.OutQuint), true);

            controlPointsVisible.BindValueChanged(visible =>
            {
                if (visible.NewValue || alwaysShowControlPoints)
                    controlPoints.FadeIn(400, Easing.OutQuint);
                else
                    controlPoints.FadeOut(200, Easing.OutQuint);
            }, true);
        }

        private void updateWaveformOpacity() =>
            waveform.FadeTo(waveformOpacity.Value, 200, Easing.OutQuint);

        private void applyVisualOffset(IBindable<WorkingBeatmap> beatmap)
        {
            waveform.RelativePositionAxes = Axes.X;

            if (beatmap.Value.Track.Length > 0)
                waveform.X = -(float)(Editor.WAVEFORM_VISUAL_OFFSET / beatmap.Value.Track.Length);
            else
            {
                // sometimes this can be the case immediately after a track switch.
                // reschedule with the hope that the track length eventually populates.
                Scheduler.AddOnce(applyVisualOffset, beatmap);
            }
        }
    }
}
