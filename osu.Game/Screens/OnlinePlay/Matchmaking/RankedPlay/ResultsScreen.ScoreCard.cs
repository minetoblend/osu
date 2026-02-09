// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Transforms;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer;
using osu.Game.Scoring;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        public partial class ScoreCard(ScoreInfo playerScore, ScoreInfo opponentScore) : CompositeDrawable
        {
            private Container content = null!;
            private Box flash = null!;

            private ScoreBar playerScoreBar = null!;
            private ScoreBar opponentScoreBar = null!;
            private FillFlowContainer roundInfo = null!;
            private RankedPlayUserDisplay playerUserDisplay = null!;
            private RankedPlayUserDisplay opponentUserDisplay = null!;

            private readonly BindableFloat scoreBarProgress = new BindableFloat();
            private readonly BindableLong damageValue = new BindableLong();

            private ScoreCounter playerScoreText = null!;
            private ScoreCounter opponentScoreText = null!;
            private ScoreDetails playerScoreDetails = null!;
            private ScoreDetails opponentScoreDetails = null!;
            private DamageDisplay damageDisplay = null!;

            private readonly Bindable<Visibility> cornerPieceVisibility = new Bindable<Visibility>(Visibility.Hidden);

            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                matchInfo.RoomState.DamageMultiplier = 2;

                int numScoreDigits = (int)Math.Ceiling(Math.Log10(Math.Max(playerScore.TotalScore, opponentScore.TotalScore)));

                BufferedContainer background;

                InternalChild = content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    Children =
                    [
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Bottom = -30 },
                            Child = background = new BufferedContainer()
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.75f,
                                Padding = new MarginPadding { Bottom = 30 },
                                Children =
                                [
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 6,
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Color4Extensions.FromHex("222228"),
                                        },
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 6,
                                        BorderThickness = 2,
                                        BorderColour = new ColourInfo
                                        {
                                            TopLeft = RankedPlayColourScheme.Blue.PrimaryDarkest.Opacity(0.25f),
                                            BottomLeft = RankedPlayColourScheme.Blue.Primary.Opacity(0.5f),
                                            TopRight = RankedPlayColourScheme.Red.PrimaryDarkest.Opacity(0.25f),
                                            BottomRight = RankedPlayColourScheme.Red.Primary.Opacity(0.5f),
                                        },
                                        Child = new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Alpha = 0,
                                            AlwaysPresent = true,
                                        },
                                    },
                                ]
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = 6,
                            Children =
                            [
                                new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    ColumnDimensions =
                                    [
                                        new Dimension(),
                                        new Dimension(GridSizeMode.Absolute, 10),
                                        new Dimension(GridSizeMode.Absolute, 50),
                                        new Dimension(GridSizeMode.Absolute, 10),
                                        new Dimension(GridSizeMode.Absolute, 50),
                                        new Dimension(GridSizeMode.Absolute, 10),
                                        new Dimension()
                                    ],
                                    RowDimensions = [new Dimension()],
                                    Padding = new MarginPadding(20) { Top = 40, Bottom = 110 },
                                    Content = new Drawable?[][]
                                    {
                                        [
                                            new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Children =
                                                [
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        Height = 60,
                                                        Anchor = Anchor.BottomCentre,
                                                        Origin = Anchor.BottomCentre,
                                                        Padding = new MarginPadding { Left = 20 },
                                                        Child = playerScoreText = new ScoreCounter(numScoreDigits)
                                                        {
                                                            Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Alpha = 0,
                                                            Spacing = new Vector2(-4),
                                                        },
                                                    },
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Padding = new MarginPadding { Bottom = 60 },
                                                        Child = playerScoreDetails = new ScoreDetails(RankedPlayColourScheme.Blue, playerScore)
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Alpha = 0,
                                                        }
                                                    }
                                                ]
                                            },
                                            null,
                                            playerScoreBar = new ScoreBar(RankedPlayColourScheme.Blue)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Height = 0,
                                                Anchor = Anchor.BottomRight,
                                                Origin = Anchor.BottomRight,
                                                Alpha = 0,
                                            },
                                            null,
                                            opponentScoreBar = new ScoreBar(RankedPlayColourScheme.Red)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Height = 0,
                                                Anchor = Anchor.BottomLeft,
                                                Origin = Anchor.BottomLeft,
                                                Alpha = 0,
                                            },
                                            null,
                                            new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Children =
                                                [
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        Height = 60,
                                                        Anchor = Anchor.BottomCentre,
                                                        Origin = Anchor.BottomCentre,
                                                        Padding = new MarginPadding { Right = 20 },
                                                        Child = opponentScoreText = new ScoreCounter(numScoreDigits)
                                                        {
                                                            Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Alpha = 0,
                                                            Spacing = new Vector2(-4),
                                                        },
                                                    },
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Padding = new MarginPadding { Bottom = 60 },
                                                        Child = opponentScoreDetails = new ScoreDetails(RankedPlayColourScheme.Red, opponentScore)
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Alpha = 0,
                                                        }
                                                    }
                                                ]
                                            },
                                        ]
                                    }
                                },
                                flash = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                },
                                playerScoreText.CreateProxy(),
                                opponentScoreText.CreateProxy(),
                                new RankedPlayCornerPiece(RankedPlayColourScheme.Blue, Anchor.BottomLeft)
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    State = { BindTarget = cornerPieceVisibility },
                                    Child = playerUserDisplay = new RankedPlayUserDisplay(playerScore.UserID, Anchor.BottomLeft, RankedPlayColourScheme.Blue)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        ManualHealth = true,
                                        Health = { Value = rankedPlayScreen.PlayerUserDisplay.Health.Value }
                                    }
                                },
                                new RankedPlayCornerPiece(RankedPlayColourScheme.Red, Anchor.BottomRight)
                                {
                                    Anchor = Anchor.BottomRight,
                                    Origin = Anchor.BottomRight,
                                    State = { BindTarget = cornerPieceVisibility },
                                    Child = opponentUserDisplay = new RankedPlayUserDisplay(opponentScore.UserID, Anchor.BottomRight, RankedPlayColourScheme.Red)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        ManualHealth = true,
                                        Health = { Value = rankedPlayScreen.OpponentUserDisplay.Health.Value }
                                    }
                                },
                            ]
                        },
                        roundInfo = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Margin = new MarginPadding(50),
                            Alpha = 0,
                            Children =
                            [
                                new OsuSpriteText
                                {
                                    Text = $"Round {matchInfo.CurrentRound}",
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = OsuFont.GetFont(size: 36, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                                },
                            ]
                        },
                        damageDisplay = new DamageDisplay
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(200, 60),
                        },
                    ]
                };

                background.Add(damageDisplay.Background.CreateProxy());
            }

            [Resolved]
            private RankedPlayScreen rankedPlayScreen { get; set; } = null!;

            [Resolved]
            private RankedPlayMatchInfo matchInfo { get; set; } = null!;

            [Resolved]
            private MultiplayerClient client { get; set; } = null!;

            public void Play()
            {
                double delay = 0;

                RankedPlayUserDisplay loserUserDisplay;

                if (playerScore.TotalScore > opponentScore.TotalScore)
                {
                    loserUserDisplay = opponentUserDisplay;
                }
                else
                {
                    loserUserDisplay = playerUserDisplay;
                }

                content.FadeIn(100)
                       .ResizeTo(0)
                       .ResizeTo(new Vector2(1, 0.05f), 600, Easing.OutExpo)
                       .Then(-300)
                       .ResizeHeightTo(1, 800, Easing.OutExpo);

                flash.Delay(150)
                     .FadeOut(600, Easing.Out);

                Scheduler.AddDelayed(() => cornerPieceVisibility.Value = Visibility.Visible, 700);

                playerScoreText.Delay(700).FadeIn(600);
                opponentScoreText.Delay(700).FadeIn(600);
                roundInfo.Delay(700).FadeIn(600);

                damageDisplay
                    .FadeOut()
                    .ResizeWidthTo(200)
                    .Delay(900)
                    .FadeIn(300)
                    .ResizeWidthTo(400, 600, Easing.OutExpo);

                delay += 1000;

                using (BeginDelayedSequence(delay))
                {
                    const double score_text_duration = 3000;

                    playerScoreText
                        .TransformValueTo((long)(playerScore.TotalScore * 0.95f), score_text_duration * 0.75)
                        .Then()
                        .TransformValueTo(playerScore.TotalScore, score_text_duration * 0.25, Easing.OutQuint);
                    opponentScoreText
                        .TransformValueTo((long)(opponentScore.TotalScore * 0.95f), score_text_duration * 0.75)
                        .Then()
                        .TransformValueTo(opponentScore.TotalScore, score_text_duration * 0.25, Easing.OutQuint);

                    long damage = (Math.Abs(playerScore.TotalScore - opponentScore.TotalScore));

                    damageDisplay.DamageCounter
                                 .TransformValueTo((long)(damage * 0.95f), score_text_duration * 0.75)
                                 .Then()
                                 .TransformValueTo(damage, score_text_duration * 0.25, Easing.OutQuint);

                    long maxAchievableScore = Math.Max(
                        Math.Max(playerScore.TotalScore, opponentScore.TotalScore),
                        1_000_000
                    );

                    float playerScorePercent = (float)playerScore.TotalScore / maxAchievableScore;
                    float opponentScorePercent = (float)opponentScore.TotalScore / maxAchievableScore;
                    float minScorePercent = Math.Min(playerScorePercent, opponentScorePercent);
                    float maxScorePercent = Math.Max(playerScorePercent, opponentScorePercent);

                    playerScoreBar.FadeIn(100);
                    opponentScoreBar.FadeIn(100);

                    this.TransformBindableTo(scoreBarProgress, maxScorePercent, score_text_duration, new CubicBezierEasingFunction(easeIn: 0.4, easeOut: 1));

                    scoreBarProgress.BindValueChanged(e =>
                    {
                        playerScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, playerScorePercent));
                        opponentScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, opponentScorePercent));

                        if (e.NewValue >= minScorePercent)
                        {
                            float progress = (e.NewValue - minScorePercent) / (maxScorePercent - minScorePercent);

                            damageValue.Value = (long)(Math.Abs(playerScore.TotalScore - opponentScore.TotalScore) * progress);
                        }
                    });
                }

                delay += 3500;

                if (matchInfo.RoomState.DamageMultiplier > 1)
                {
                    using (BeginDelayedSequence(delay))
                    {
                        damageDisplay.DamageCounter
                                     .ScaleTo(0.8f, 50, Easing.Out)
                                     .Then()
                                     .Schedule(() => damageDisplay.DamageCounter.SetValueInstantly((long)(Math.Abs(playerScore.TotalScore - opponentScore.TotalScore)
                                                                                                          * matchInfo.RoomState.DamageMultiplier)))
                                     .ScaleTo(1, 600, Easing.OutElasticHalf);

                        damageDisplay.MultiplierText.Delay(50)
                                     .FadeIn()
                                     .ScaleTo(0.8f)
                                     .ScaleTo(1f, 600, Easing.OutElasticHalf)
                                     .Delay(300)
                                     .FadeOut(1000, Easing.Out);
                    }

                    delay += 500;
                }

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() =>
                    {
                        foreach (var (userId, userInfo) in matchInfo.RoomState.Users)
                        {
                            if (userId == client.LocalUser!.UserID)
                                playerUserDisplay.Health.Value = userInfo.Life;
                            else
                                opponentUserDisplay.Health.Value = userInfo.Life;
                        }
                    });
                }

                delay += 400;

                using (BeginDelayedSequence(delay))
                {
                    playerScoreDetails.FadeIn(300);
                    opponentScoreDetails.FadeIn(300);
                }
            }

            public override void Hide()
            {
                cornerPieceVisibility.Value = Visibility.Hidden;

                content
                    .MoveToY(100, 400, Easing.InCubic)
                    .ScaleTo(0.8f, 400, Easing.InCubic)
                    .FadeOut(200, Easing.Out);
            }
        }
    }
}
