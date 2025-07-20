// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.SelectV2;
using osuTK;
using osuTK.Input;

namespace osu.Game.Overlays.Polls
{
    [Cached]
    public partial class PollWedge : CompositeDrawable
    {
        public readonly BindableBool Expanded = new BindableBool();

        public readonly Bindable<APIPoll?> Poll = new Bindable<APIPoll?>();

        private float transitionProgress { get; set; }

        private Container scaleContainer = null!;
        private WedgeContent expandedContent = null!;

        private OsuTextFlowContainer topicText = null!;
        private OsuSpriteText voteCountText = null!;
        private Container wedgeBackground = null!;
        private Container buttonBackground = null!;
        private WedgeContent header = null!;

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        [Resolved]
        private PollOverlay pollOverlay { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Shear = OsuGame.SHEAR;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                scaleContainer = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        buttonBackground = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = ShearedButton.CORNER_RADIUS,
                            BorderThickness = ShearedButton.BORDER_THICKNESS,
                            BorderColour = ColourInfo.GradientVertical(colourProvider.Background5, colourProvider.Background4),
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = colourProvider.Background5,
                            },
                        },
                        wedgeBackground = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = ShearedButton.CORNER_RADIUS,
                            Child = new WedgeBackground()
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            AutoSizeDuration = 300,
                            AutoSizeEasing = Easing.OutExpo,
                            Direction = FillDirection.Vertical,
                            Masking = true,
                            CornerRadius = ShearedButton.CORNER_RADIUS,
                            Children = new Drawable[]
                            {
                                header = new WedgeContent
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Padding = new MarginPadding { Horizontal = 15, Vertical = 10 },
                                    Children = new Drawable[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Text = "Today's poll!",
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Shear = -OsuGame.SHEAR,
                                            Font = OsuFont.Style.Heading2,
                                            Colour = colourProvider.Light1,
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Shear = -OsuGame.SHEAR,
                                            Spacing = new Vector2(3),
                                            Children = new Drawable[]
                                            {
                                                voteCountText = new OsuSpriteText
                                                {
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Font = OsuFont.Style.Heading2,
                                                    Colour = colourProvider.Light1,
                                                },
                                                new SpriteIcon
                                                {
                                                    Size = new Vector2(OsuFont.Style.Heading2.Size * 0.8f),
                                                    Icon = FontAwesome.Solid.User,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Colour = colourProvider.Light1,
                                                }
                                            }
                                        },
                                    },
                                },
                                expandedContent = new WedgeContent
                                {
                                    Margin = new MarginPadding { Top = 10 },
                                    AutoSizeAxes = Axes.Both,
                                    Background = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = ShearedButton.CORNER_RADIUS,
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = colourProvider.Background6,
                                            Alpha = 0.5f,
                                        }
                                    },
                                    Child = topicText = new OsuTextFlowContainer(s =>
                                    {
                                        s.Shear = -OsuGame.SHEAR;
                                        s.Font = OsuFont.Style.Body.With(weight: FontWeight.Medium);
                                        s.Margin = new MarginPadding { Right = -4 };
                                    })
                                    {
                                        Width = 350,
                                        Padding = new MarginPadding { Right = 50, Vertical = 10 },
                                        AutoSizeAxes = Axes.Y,
                                        Text = "Should we bring back green lines?",
                                    }
                                },
                            }
                        }
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Poll.BindValueChanged(e =>
            {
                if (e.NewValue is not APIPoll poll)
                    return;

                topicText.Text = poll.Topic;
                voteCountText.Text = poll.TotalVoteCount.ToString();
            }, true);

            Expanded.BindValueChanged(e =>
            {
                const float duration = 300;
                const Easing easing = Easing.OutExpo;

                this.TransformTo(nameof(transitionProgress), e.NewValue ? 1f : 0f, duration, easing)
                    .ResizeWidthTo(e.NewValue ? 350 : 180, duration, easing);

                if (e.NewValue)
                {
                    header.TransformTo(nameof(Padding), new MarginPadding { Right = 15, Vertical = 10 });
                    wedgeBackground.FadeIn(duration, Easing.Out);
                    buttonBackground.FadeOut(duration, Easing.Out);
                    expandedContent.FadeIn(duration, Easing.Out);
                    expandedContent.BypassAutoSizeAxes = Axes.None;
                }
                else
                {
                    header.TransformTo(nameof(Padding), new MarginPadding { Horizontal = 15, Vertical = 10 });
                    wedgeBackground.FadeOut(duration, Easing.Out);
                    buttonBackground.FadeIn(duration, Easing.Out);
                    expandedContent.FadeOut(duration, Easing.Out);
                    expandedContent.BypassAutoSizeAxes = Axes.Both;
                }
            }, true);
            FinishTransforms(true);
        }

        public override bool HandlePositionalInput => !Expanded.Value;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button == MouseButton.Left)
            {
                scaleContainer.ClearTransforms();
                scaleContainer.ScaleTo(0.95f, 300, Easing.OutExpo);
            }

            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (e.Button == MouseButton.Left)
            {
                scaleContainer.ClearTransforms();
                scaleContainer.ScaleTo(1, 400, Easing.OutElasticHalf);
            }
        }

        protected override bool OnClick(ClickEvent e)
        {
            Expanded.Value = true;
            return true;
        }

        public float ShearOffset
        {
            get
            {
                float offset = pollOverlay.ToLocalSpace(ScreenSpaceDrawQuad.TopLeft).X;

                return (offset + ShearedButton.CORNER_RADIUS) * transitionProgress;
            }
        }

        protected override void Update()
        {
            base.Update();

            Padding = new MarginPadding { Left = -ShearOffset };
        }

        public partial class WedgeContent : Container
        {
            public Drawable Background
            {
                init => backgroundContainer.Child = value;
            }

            private readonly Container backgroundContainer;
            private readonly Container offsetContainer;

            protected override Container<Drawable> Content { get; }

            public new MarginPadding Padding
            {
                get => Content.Padding;
                set => Content.Padding = value;
            }

            public WedgeContent()
            {
                InternalChildren = new Drawable[]
                {
                    backgroundContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    offsetContainer = new Container
                    {
                        Child = Content = new Container(),
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                offsetContainer.RelativeSizeAxes = Content.RelativeSizeAxes = RelativeSizeAxes;
                offsetContainer.AutoSizeAxes = Content.AutoSizeAxes = AutoSizeAxes;
            }

            [Resolved]
            private PollWedge wedge { get; set; } = null!;

            protected override void Update()
            {
                base.Update();

                offsetContainer.Padding = new MarginPadding { Left = wedge.ShearOffset };
            }
        }
    }
}
