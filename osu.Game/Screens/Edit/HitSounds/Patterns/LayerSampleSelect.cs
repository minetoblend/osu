// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class LayerSampleSelect : CompositeDrawable
    {
        private readonly HitSoundLayer layer;

        private readonly Bindable<string> sampleName;

        private TruncatingSpriteText spriteText = null!;

        public LayerSampleSelect(HitSoundLayer layer)
        {
            this.layer = layer;

            sampleName = layer.NameBindable.GetBoundCopy();
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            Masking = true;
            Size = new Vector2(140, 26);
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background5,
                },
                spriteText = new TruncatingSpriteText
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = OsuFont.GetFont(size: 16),
                    Padding = new MarginPadding
                    {
                        Horizontal = 10
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            sampleName.BindValueChanged(name => spriteText.Text = name.NewValue, true);
        }
    }
}
