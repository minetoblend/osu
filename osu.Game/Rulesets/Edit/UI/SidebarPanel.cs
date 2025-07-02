// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Edit.UI
{
    public partial class SidebarPanel : Container
    {
        private LocalisableString title;

        public LocalisableString Title
        {
            get => title;
            set
            {
                if (title == value)
                    return;

                header.Text = value;

                if (LoadState >= LoadState.Ready)
                    header.Text = value;
            }
        }

        public readonly BindableBool Expanded = new BindableBool(true);

        private PanelHeader header = null!;
        private Box background = null!;

        private readonly FillFlowContainer content = new FillFlowContainer
        {
            Name = @"Content",
            Origin = Anchor.TopCentre,
            Anchor = Anchor.TopCentre,
            Direction = FillDirection.Vertical,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding { Horizontal = 10, Top = 5, Bottom = 10 },
            Spacing = new Vector2(0, 15),
        };

        protected sealed override Container<Drawable> Content => content;

        public SidebarPanel(LocalisableString title)
        {
            this.title = title;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider? colourProvider)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Masking = true;
            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.1f,
                    Colour = colourProvider?.Background4 ?? Color4.Black,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding(10),
                    Spacing = new Vector2(0, 5),
                    Children = new Drawable[]
                    {
                        header = new PanelHeader
                        {
                            Text = title,
                            Expanded = { BindTarget = Expanded },
                        },
                        content
                    },
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Expanded.BindValueChanged(e =>
            {
                Content.BypassAutoSizeAxes = e.NewValue ? Axes.None : Axes.Y;
                Content.FadeTo(e.NewValue ? 1 : 0, 200, Easing.Out);
            }, true);
        }

        protected override void UpdateAfterAutoSize()
        {
            base.UpdateAfterAutoSize();

            AutoSizeDuration = 300;
            AutoSizeEasing = Easing.OutExpo;
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateFadeState();
            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateFadeState();

            base.OnHoverLost(e);
        }

        private void updateFadeState()
        {
            const float fade_duration = 500;

            background.FadeTo(IsHovered ? 1 : 0.1f, fade_duration, Easing.OutQuint);
        }

        private partial class PanelHeader : CompositeDrawable
        {
            public required LocalisableString Text
            {
                get => text.Text;
                set => text.Text = value.ToUpper();
            }

            private readonly OsuSpriteText text;
            private readonly SpriteIcon icon;

            public readonly BindableBool Expanded = new BindableBool();

            public PanelHeader()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChild = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(4),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            Size = new Vector2(10),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Child = icon = new SpriteIcon
                            {
                                RelativeSizeAxes = Axes.Both,
                                Icon = FontAwesome.Solid.ChevronRight,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            },
                        },
                        text = new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Padding = new MarginPadding { Vertical = 4 },
                            Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 17),
                        }
                    }
                };
            }

            [Resolved]
            private OverlayColourProvider colourProvider { get; set; } = null!;

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Expanded.BindValueChanged(e =>
                {
                    icon.RotateTo(e.NewValue ? 90 : 0, 300, Easing.OutExpo);
                    this.FadeTo(e.NewValue ? 1 : 0.5f, 300, Easing.Out);
                }, true);
                FinishTransforms(true);
            }

            protected override bool OnClick(ClickEvent e)
            {
                Expanded.Toggle();

                return true;
            }
        }
    }
}
