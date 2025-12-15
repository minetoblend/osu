// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components
{
    public partial class RankedPlayUserDisplay : CompositeDrawable
    {
        private readonly APIUser user;

        public readonly BindableInt Health = new BindableInt
        {
            MaxValue = 1_000_000,
            MinValue = 0,
            Value = 1_000_000,
        };

        public RankedPlayUserDisplay(APIUser user, Anchor contentAnchor, RankedPlayColourScheme colourScheme)
        {
            this.user = user;

            InternalChildren =
            [
                new CircularContainer
                {
                    Name = "Avatar",
                    Size = new Vector2(72),
                    Masking = true,
                    Anchor = contentAnchor,
                    Origin = contentAnchor,
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourScheme.Surface,
                            Alpha = 0.5f,
                        },
                        new UpdateableAvatar(user)
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    ]
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = (contentAnchor & Anchor.x0) != 0 ? new MarginPadding { Left = 72 } : new MarginPadding { Right = 72 },
                    Direction = FillDirection.Vertical,
                    Children =
                    [
                        new HealthBar(colourScheme, (contentAnchor & Anchor.x0) != 0)
                        {
                            Health = { BindTarget = Health },
                            RelativeSizeAxes = Axes.X,
                            Height = 22,
                            Anchor = contentAnchor,
                            Origin = contentAnchor,
                        },
                        new OsuSpriteText
                        {
                            Name = "Username",
                            Text = user.Username,
                            Anchor = contentAnchor,
                            Origin = contentAnchor,
                            Padding = new MarginPadding { Horizontal = 4, Vertical = 6 },
                            Font = OsuFont.GetFont(size: 24, weight: FontWeight.SemiBold),
                            UseFullGlyphHeight = false,
                        },
                    ]
                }
            ];
        }

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.RoomUpdated += onRoomUpdated;
        }

        private void onRoomUpdated()
        {
            if (client.Room?.Users.FirstOrDefault(it => it.UserID == this.user.Id)?.MatchState is not RankedPlayUserState matchState)
                return;

            Health.Value = matchState.Life;
        }

        protected override void Dispose(bool isDisposing)
        {
            client.RoomUpdated -= onRoomUpdated;

            base.Dispose(isDisposing);
        }

        private partial class HealthBar : CompositeDrawable
        {
            public readonly BindableInt Health = new BindableInt
            {
                MaxValue = 1_000_000,
                MinValue = 0,
                Value = 1_000_000,
            };

            private readonly BindableInt healthDelayed = new BindableInt();

            /// <summary>
            /// relative health threshold below which the health bar starts flashing red
            /// </summary>
            public float HealthFlashThreshold { get; set; } = 0.3f;

            private readonly ColourInfo healthBarColour;
            private readonly Container healthBar;
            private readonly Box healthBarBackground;
            private readonly TrianglesV2 triangles;
            private readonly SpriteIcon heartIcon;
            private readonly OsuSpriteText healthText;

            public HealthBar(RankedPlayColourScheme colourScheme, bool leftToRight)
            {
                Shear = OsuGame.SHEAR;

                Anchor contentAnchor = leftToRight ? Anchor.CentreLeft : Anchor.CentreRight;

                InternalChildren =
                [
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 3,
                        BorderThickness = 1f,
                        BorderColour = ColourInfo.GradientVertical(colourScheme.Surface, colourScheme.SurfaceBorder),
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourScheme.Surface,
                            Alpha = 0.8f,
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Horizontal = 2.2f, Vertical = 2 }, // slightly different ratio to account for shear
                        Child = healthBar = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = 2,
                            Anchor = contentAnchor,
                            Origin = contentAnchor,
                            Children =
                            [
                                healthBarBackground = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Alpha = 0.8f,
                                    Colour = healthBarColour = leftToRight
                                        ? ColourInfo.GradientHorizontal(colourScheme.PrimaryDarker, colourScheme.Primary)
                                        : ColourInfo.GradientHorizontal(colourScheme.Primary, colourScheme.PrimaryDarker),
                                },
                                triangles = new TrianglesV2
                                {
                                    RelativeSizeAxes = Axes.Y,
                                    Anchor = contentAnchor,
                                    Origin = contentAnchor,
                                    SpawnRatio = 0.5f,
                                    ScaleAdjust = 0.75f,
                                    Alpha = 0.1f,
                                    Blending = BlendingParameters.Additive,
                                    Colour = leftToRight
                                        ? ColourInfo.GradientHorizontal(Color4.Transparent, Color4.White)
                                        : ColourInfo.GradientHorizontal(Color4.White, Color4.Transparent),
                                }
                            ]
                        },
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Shear = -OsuGame.SHEAR,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Spacing = new Vector2(3),
                        Padding = new MarginPadding { Horizontal = 10 },
                        Children =
                        [
                            new Container
                            {
                                Size = new Vector2(10),
                                Anchor = contentAnchor,
                                Origin = contentAnchor,
                                Child = heartIcon = new SpriteIcon
                                {
                                    Icon = FontAwesome.Solid.Heart,
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                }
                            },
                            healthText = new OsuSpriteText
                            {
                                Text = "1,000,000",
                                Anchor = contentAnchor,
                                Origin = contentAnchor,
                                Font = OsuFont.GetFont(size: 14, weight: FontWeight.Medium),
                                UseFullGlyphHeight = false,
                                Padding = new MarginPadding { Top = 1 }
                            }
                        ]
                    }
                ];
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Health.BindValueChanged(e =>
                {
                    healthBar.ResizeWidthTo(Health.NormalizedValue, 400, Easing.OutExpo);

                    this.TransformBindableTo(healthDelayed, e.NewValue, 300, Easing.OutExpo);
                }, true);

                healthDelayed.BindValueChanged(e => healthText.Text = FormattableString.Invariant($"{e.NewValue:N0}"), true);

                FinishTransforms(true);

                Scheduler.AddDelayed(flashHealth, 1000, true);
            }

            protected override void Update()
            {
                base.Update();

                triangles.Width = DrawWidth;
            }

            private void flashHealth()
            {
                if (Health.NormalizedValue > HealthFlashThreshold)
                    return;

                var flashColour = Interpolation.ValueAt(0.75, healthBarColour, ColourInfo.SingleColour(Color4.Red), 0.0, 1.0);

                healthBarBackground.FadeColour(flashColour, 150)
                                   .Then()
                                   .FadeColour(healthBarColour, 800);

                heartIcon
                    .ScaleTo(0.8f, 150, Easing.Out)
                    .Then()
                    .ScaleTo(1f, 400, Easing.OutElasticHalf);
            }
        }
    }
}
