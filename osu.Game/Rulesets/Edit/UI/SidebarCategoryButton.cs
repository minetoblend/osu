// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Edit.UI
{
    public partial class SidebarCategoryButton : CompositeDrawable
    {
        private readonly SidebarCategory category;
        private readonly Bindable<SidebarCategory?> activeCategory;

        private readonly Bindable<bool> active = new Bindable<bool>();

        private VerticalText text = null!;
        private Box background = null!;

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        public SidebarCategoryButton(SidebarCategory category, Bindable<SidebarCategory?> activeCategory)
        {
            this.category = category;
            this.activeCategory = activeCategory.GetBoundCopy();
        }

        [BackgroundDependencyLoader]
        private void load(Editor? editor)
        {
            AutoSizeAxes = Axes.Both;
            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4
                },
                text = new VerticalText
                {
                    Text = category.Name,
                    Margin = new MarginPadding { Horizontal = 4, Vertical = 10 }
                },
                new HoverHighlight(),
                new HoverClickSounds(HoverSampleSet.TabSelect)
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activeCategory.BindValueChanged(c => active.Value = c.NewValue == category, true);

            active.BindValueChanged(_ => updateState(), true);
        }

        private void updateState()
        {
            background.FadeColour(active.Value ? colourProvider.Background3 : colourProvider.Background5, 100);
            text.FadeColour(active.Value ? Color4.White : colourProvider.Light3, 100);
        }

        private void onActivated()
        {
            background.FadeColour(colourProvider.Background3);
            text.FadeColour(Color4.White, 100);
        }

        private void onDeactivated()
        {
            background.FadeColour(colourProvider.Background4);
            text.FadeColour(colourProvider.Light3, 100);
        }

        protected override bool OnClick(ClickEvent e)
        {
            activeCategory.Value = activeCategory.Value == category ? null : category;

            return true;
        }

        private partial class VerticalText : CompositeDrawable
        {
            public LocalisableString Text
            {
                get => spriteText.Text;
                set => spriteText.Text = value;
            }

            private readonly OsuSpriteText spriteText;

            public VerticalText()
            {
                InternalChild = spriteText = new OsuSpriteText
                {
                    Rotation = 90,
                    Anchor = Anchor.TopRight,
                };
            }

            protected override void Update()
            {
                base.Update();

                Size = new Vector2(spriteText.LayoutSize.Y, spriteText.LayoutSize.X);
            }
        }
    }
}
