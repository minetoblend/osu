// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Screens.Edit.Timing.Blueprints;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingPointLayer : ControlPointLayer<TimingControlPoint, ControlPointBlueprint>
    {
        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Resolved]
        private LayeredTimeline timeline { get; set; } = null!;

        public TimingPointLayer()
            : base("Timing")
        {
        }

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        protected override IReadOnlyList<TimingControlPoint> GetControlPointList(ControlPointInfo controlPointInfo) => controlPointInfo.TimingPoints;

        public override Color4 LayerColour => colours.Red2;

        private readonly IBindable<Track> track = new Bindable<Track>();

        private WaveformGraph waveform = null!;

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap)
        {
            Add(waveform = new WaveformGraph
            {
                RelativeSizeAxes = Axes.Both,
                BaseColour = colours.Blue.Opacity(0.2f),
                LowColour = colours.BlueLighter,
                MidColour = colours.BlueDark,
                HighColour = colours.BlueDarker,
                Alpha = 0.25f
            });

            track.BindTo(editorClock.Track);
            track.BindValueChanged(_ =>
            {
                waveform.Waveform = beatmap.Value.Waveform;
            }, true);
        }

        private readonly ControlPointBlueprintContainer blueprintContainer = new ControlPointBlueprintContainer();

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            timeline.BackgroundLayer.Add(waveform.CreateProxy());
        }
    }
}
