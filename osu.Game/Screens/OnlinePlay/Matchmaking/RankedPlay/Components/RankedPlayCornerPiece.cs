// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components
{
    public partial class RankedPlayCornerPiece : VisibilityContainer
    {
        private readonly Container background;

        protected override Container<Drawable> Content { get; }

        public RankedPlayCornerPiece(RankedPlayColourScheme colourScheme, Anchor anchor)
        {
            Size = new Vector2(345, 100);

            Anchor = Origin = anchor;

            InternalChildren =
            [
                background = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(
                        (anchor & Anchor.x0) != 0 ? 1 : -1,
                        (anchor & Anchor.y0) != 0 ? -1 : 1
                    ),
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Rotation = -2,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Shear = new Vector2(-0.5f, 0),
                        Padding = new MarginPadding
                        {
                            Left = -60,
                            Bottom = -30,
                            Top = 20,
                            Right = 15,
                        },
                        Children =
                        [
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Masking = true,
                                CornerRadius = 20,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = colourScheme.PrimaryDarkest,
                                    Alpha = 0.2f,
                                },
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding(10),
                                Child = new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Masking = true,
                                    CornerRadius = 15,
                                    Child = new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = ColourInfo.GradientHorizontal(colourScheme.Primary.Opacity(0.75f), colourScheme.PrimaryDarker.Opacity(0.25f)),
                                    },
                                },
                            }
                        ]
                    },
                },
                Content = new Container
                {
                    Anchor = anchor,
                    Origin = anchor,
                    Margin = new MarginPadding(18),
                    RelativeSizeAxes = Axes.Both,
                }
            ];
        }

        protected override void Update()
        {
            base.Update();

            Width = WidthFor(Parent!.ChildSize.X);
        }

        public static float WidthFor(float parentWidth) => float.Clamp(parentWidth * 0.25f, 250, 335);

        protected override void PopIn()
        {
            this.FadeIn(300);

            Content.MoveToX(0, 500, Easing.OutExpo);
            background.MoveToY(0, 500, Easing.OutExpo);
        }

        protected override void PopOut()
        {
            this.FadeOut(500);

            background.MoveToY((Anchor & Anchor.y0) != 0 ? -120 : 120, 500, new CubicBezierEasingFunction(easeIn: 0.2, easeOut: 0.75));
            Content.MoveToX((Anchor & Anchor.x0) != 0 ? -500 : 500, 500, new CubicBezierEasingFunction(easeIn: 0.33, easeOut: 0.5));
        }
    }
}
