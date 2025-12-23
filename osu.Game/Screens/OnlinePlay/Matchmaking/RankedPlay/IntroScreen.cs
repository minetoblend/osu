// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays.Dashboard;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class IntroScreen : RankedPlaySubScreen
    {
        public IntroScreen()
        {
            CornerPieceVisibility.Value = Visibility.Hidden;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Box box;
            Drawable user1, user2;
            StarRatingRangeDisplay rangeDisplay;

            InternalChildren =
            [
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Shear = OsuGame.SHEAR,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children =
                    [
                        box = new Box
                        {
                            RelativeSizeAxes = Axes.Y,
                            Height = 2,
                            Width = 4,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        user1 = new CurrentlyOnlineDisplay.OnlineUserPanel(new APIUser
                        {
                            Id = 0,
                            Username = "Hydrogen Bomb",
                        })
                        {
                            RelativePositionAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Position = new Vector2(-0.25f, -0.75f),
                            Shear = -OsuGame.SHEAR,
                        },
                        user2 = new CurrentlyOnlineDisplay.OnlineUserPanel(new APIUser
                        {
                            Id = 1,
                            Username = "Coughing Baby",
                        })
                        {
                            RelativePositionAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Position = new Vector2(0.25f, 0.75f),
                            Shear = -OsuGame.SHEAR,
                        },
                    ],
                },
                rangeDisplay = new StarRatingRangeDisplay(starRating: 6.5)
                {
                    Alpha = 0,
                }
            ];

            box.ResizeHeightTo(0)
               .ResizeHeightTo(1f, 1000, Easing.OutExpo);

            user1.Delay(500)
                 .MoveToY(-0.15f, 1000, Easing.OutExpo);

            user2.Delay(500)
                 .MoveToY(0.15f, 1000, Easing.OutExpo);

            Scheduler.AddDelayed(() =>
            {
                user1.MoveToY(1, 600, Easing.InCubic);
                user2.MoveToY(-1, 600, Easing.InCubic);
                box.Delay(100).ResizeHeightTo(0, 400, Easing.InQuad);
            }, 2000);

            Scheduler.AddDelayed(() =>
            {
                CornerPieceVisibility.Value = Visibility.Visible;
            }, 2600);

            Scheduler.AddDelayed(rangeDisplay.PlayAnimation, 2700);
        }

        private partial class StarRatingRangeDisplay : CompositeDrawable
        {
            private readonly double starRating;

            public StarRatingRangeDisplay(double starRating)
            {
                this.starRating = starRating;

                RelativeSizeAxes = Axes.Both;
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
            }

            private Box leftBar = null!;
            private Box rightBar = null!;
            private OsuSpriteText title = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new FillFlowContainer
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.6f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(10),
                    Children =
                    [
                        title = new OsuSpriteText
                        {
                            Text = "Finding Match Rating...",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.Style.Heading1,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 20,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children =
                            [
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.LightGray,
                                },
                                leftBar = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.DarkGray,
                                    Width = 0,
                                },
                                rightBar = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.DarkGray,
                                    Width = 0,
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                }
                            ]
                        }
                    ],
                };
            }

            public void PlayAnimation()
            {
                this.FadeIn(300)
                    .ScaleTo(0.8f)
                    .ScaleTo(1, 400, Easing.OutExpo);

                const double sr_min = 2;
                const double sr_max = 9;

                float progress = (float)((starRating - sr_min) / (sr_max - sr_min));

                using (BeginDelayedSequence(500))
                {
                    leftBar.ResizeWidthTo(progress - 0.05f, 1500, Easing.OutCubic);
                    rightBar.ResizeWidthTo(1 - progress - 0.05f, 1500, Easing.OutCubic);
                }

                Scheduler.AddDelayed(() =>
                {
                    title.Text = "Match rating found!";
                }, 2500);
            }
        }
    }
}
