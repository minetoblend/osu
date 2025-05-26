// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.HitSounds.Patterns;

namespace osu.Game.Screens.Edit.HitSounds
{
    public partial class HitSoundComposer : CompositeDrawable
    {
        [Resolved]
        private Bindable<WorkingBeatmap> beatmap { get; set; } = null!;

        private readonly Bindable<HitSoundPattern> activePattern = new Bindable<HitSoundPattern>();

        private Container toolContainer = null!;

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background6,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = PatternLayer.HEADER_WIDTH,
                    Colour = colourProvider.Background2
                },
                toolContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activePattern.BindValueChanged(pattern =>
            {
                if (pattern.NewValue != null)
                    toolContainer.Child = new PatternEditor(pattern.NewValue);
                else
                    toolContainer.Clear();
            }, true);

            activePattern.Value = new HitSoundPattern
            {
                Layers =
                {
                    new HitSoundLayer
                    {
                        Name = "normal-hitnormal",
                    },
                    new HitSoundLayer
                    {
                        Name = "normal-hitwhistle"
                    },
                    new HitSoundLayer
                    {
                        Name = "normal-hitfinish"
                    },
                    new HitSoundLayer
                    {
                        Name = "normal-hitclap"
                    },
                },
                HitSounds = new[]
                {
                    new HitSound { StartTime = 500 },
                    new HitSound { StartTime = 1000, Layer = 1 },
                }
            };
        }

        protected override bool OnMidiDown(MidiDownEvent e)
        {
            return base.OnMidiDown(e);
        }
    }
}
