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
using osu.Framework.Graphics.Transforms;
using osu.Game.Extensions;
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
            private OsuSpriteText victoryText = null!;
            private Container playerScoreContainer = null!;
            private Container opponentScoreContainer = null!;
            private FillFlowContainer roundInfo = null!;
            private RankedPlayUserDisplay playerUserDisplay = null!;
            private RankedPlayUserDisplay opponentUserDisplay = null!;

            private readonly BindableLong playerScoreValue = new BindableLong();
            private readonly BindableLong opponentScoreValue = new BindableLong();
            private readonly BindableFloat scoreBarProgress = new BindableFloat();
            private readonly BindableLong damageValue = new BindableLong();

            private ScoreCounter playerScoreText = null!;
            private ScoreCounter opponentScoreText = null!;
            private OsuSpriteText damageText = null!;
            private OsuSpriteText damageMultiplierText = null!;
            private Container damageTextContainer = null!;

            private readonly Bindable<Visibility> cornerPieceVisibility = new Bindable<Visibility>(Visibility.Hidden);

            [BackgroundDependencyLoader]
            private void load()
            {
                matchInfo.RoomState.DamageMultiplier = 2;

                int numScoreDigits = (int)Math.Ceiling(Math.Log10(Math.Max(playerScore.TotalScore, opponentScore.TotalScore)));

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
                            Masking = true,
                            CornerRadius = 6,
                            Children =
                            [
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4Extensions.FromHex("222228"),
                                    Alpha = 0.75f,
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
                                                        },
                                                    },
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Padding = new MarginPadding { Bottom = 60 },
                                                        Child = playerScoreContainer = new Container
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Masking = true,
                                                            CornerRadius = 6,
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
                                                        },
                                                    },
                                                    new Container
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Padding = new MarginPadding { Bottom = 60 },
                                                        Child = opponentScoreContainer = new Container
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                            Masking = true,
                                                            CornerRadius = 6,
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
                            Margin = new MarginPadding(30),
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
                                new OsuSpriteText
                                {
                                    Text = $"Damage {matchInfo.RoomState.DamageMultiplier}x",
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = OsuFont.GetFont(size: 24, weight: FontWeight.SemiBold, typeface: Typeface.Torus),
                                    Alpha = 0.8f
                                },
                            ]
                        },
                        victoryText = new OsuSpriteText
                        {
                            Text = "Victory",
                            Font = OsuFont.GetFont(size: 48, weight: FontWeight.SemiBold, typeface: Typeface.TorusAlternate),
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.Centre,
                            Y = -6,
                            Spacing = new Vector2(-10),
                            Alpha = 0,
                        },
                        damageTextContainer = new Container
                        {
                            Size = new Vector2(300, 60),
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Margin = new MarginPadding(30),
                            Alpha = 0,
                            Children =
                            [
                                damageText = new OsuSpriteText
                                {
                                    Text = "Damage: 000,000",
                                    Font = OsuFont.GetFont(size: 32, fixedWidth: true),
                                    Colour = RankedPlayColourScheme.Red.Primary,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Shadow = false,
                                },
                                damageMultiplierText = new OsuSpriteText
                                {
                                    BypassAutoSizeAxes = Axes.Both,
                                    Text = $"{matchInfo.RoomState.DamageMultiplier.ToStandardFormattedString(maxDecimalDigits: 1)}x",
                                    Font = OsuFont.GetFont(size: 64, fixedWidth: true, weight: FontWeight.Bold),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Y = -20,
                                    X = 50,
                                    Rotation = 25,
                                    Shadow = false,
                                    Alpha = 0,
                                }
                            ]
                        },
                    ]
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                damageValue.BindValueChanged(v => damageText.Text = $"Damage: {v.NewValue:000,000}");
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
                // damageTextContainer.Delay(700).FadeIn(600);

                delay += 1000;

                using (BeginDelayedSequence(delay))
                {
                    const double score_text_duration = 3000;

                    playerScoreText.TransformValueTo(playerScore.TotalScore, score_text_duration, Easing.OutQuint);
                    opponentScoreText.TransformValueTo(opponentScore.TotalScore, score_text_duration, Easing.OutQuint);

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

                            long damage = (long)(Math.Abs(playerScore.TotalScore - opponentScore.TotalScore) * progress);
                            damageValue.Value = damage;
                        }
                    });
                }

                delay += 2000;

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() =>
                    {
                        playerScoreContainer.Add(new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children =
                            [
                            ]
                        });
                        opponentScoreContainer.Add(new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children =
                            [
                            ]
                        });
                        playerScoreContainer.FadeIn(600);
                        opponentScoreContainer.FadeIn(600);
                    });

                    victoryText.Text = playerScore.TotalScore > opponentScore.TotalScore ? "Victory" : "Defeat";
                }

                delay += 1400;

                using (BeginDelayedSequence(delay))
                {
                    long damage = (long)(Math.Abs(playerScore.TotalScore - opponentScore.TotalScore) * matchInfo.RoomState.DamageMultiplier);

                    damageText.ScaleTo(0.8f, 60)
                              .Then()
                              .Schedule(() => damageValue.Value = damage)
                              .ScaleTo(1f, 500, Easing.OutElasticHalf);

                    damageMultiplierText.Delay(60f)
                                        .FadeIn()
                                        .ScaleTo(0.5f)
                                        .FadeOut(600)
                                        .ScaleTo(1.2f, 600, Easing.OutExpo);
                }

                delay += 1200;

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() =>
                    {
                        damageTextContainer.MoveTo(damageTextContainer.Parent!.ToLocalSpace(loserUserDisplay.HealthDisplay.ScreenSpaceDrawQuad.Centre) - damageTextContainer.AnchorPosition, 400,
                                               Easing.InCubic)
                                           .ScaleTo(0.5f, 400, Easing.InCubic)
                                           .Then()
                                           .FadeOut();
                    });
                }

                delay += 400;

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
