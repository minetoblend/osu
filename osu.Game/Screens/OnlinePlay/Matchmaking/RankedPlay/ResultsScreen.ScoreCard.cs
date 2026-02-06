// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
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
            private OsuSpriteText victoryText = null!;
            private Container playerScoreContainer = null!;
            private Container opponentScoreContainer = null!;

            private readonly BindableLong playerScoreValue = new BindableLong();
            private readonly BindableLong opponentScoreValue = new BindableLong();
            private readonly BindableFloat scoreBarProgress = new BindableFloat();

            private OsuSpriteText playerScoreText = null!;
            private OsuSpriteText opponentScoreText = null!;

            private readonly Bindable<Visibility> cornerPieceVisibility = new Bindable<Visibility>(Visibility.Hidden);

            [BackgroundDependencyLoader]
            private void load()
            {
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
                                                        Child = playerScoreText = new OsuSpriteText
                                                        {
                                                            Text = "0,000,000",
                                                            Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Shadow = false,
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
                                                        Child = opponentScoreText = new OsuSpriteText
                                                        {
                                                            Text = "0,000,000",
                                                            Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            Shadow = false,
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
                                },
                                new RankedPlayCornerPiece(RankedPlayColourScheme.Blue, Anchor.BottomLeft)
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    State = { BindTarget = cornerPieceVisibility },
                                    Child = new RankedPlayUserDisplay(playerScore.UserID, Anchor.BottomLeft, RankedPlayColourScheme.Blue)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                    }
                                },
                                new RankedPlayCornerPiece(RankedPlayColourScheme.Red, Anchor.BottomRight)
                                {
                                    Anchor = Anchor.BottomRight,
                                    Origin = Anchor.BottomRight,
                                    State = { BindTarget = cornerPieceVisibility },
                                    Child = new RankedPlayUserDisplay(opponentScore.UserID, Anchor.BottomRight, RankedPlayColourScheme.Red)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                    }
                                },
                            ]
                        },
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Margin = new MarginPadding(30),
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
                    ]
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                playerScoreValue.BindValueChanged(v => playerScoreText.Text = $"{v.NewValue:0,000,000}");
                opponentScoreValue.BindValueChanged(v => opponentScoreText.Text = $"{v.NewValue:0,000,000}");
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

                content.FadeIn(100)
                       .ResizeTo(0)
                       .ResizeTo(new Vector2(1, 0.05f), 600, Easing.OutExpo)
                       .Then(-300)
                       .ResizeHeightTo(1, 800, Easing.OutExpo);

                flash.FadeOut(600, Easing.Out);

                Scheduler.AddDelayed(() => cornerPieceVisibility.Value = Visibility.Visible, 700);

                playerScoreText.Delay(700).FadeIn(600);
                opponentScoreText.Delay(700).FadeIn(600);

                delay += 1000;

                using (BeginDelayedSequence(delay))
                {
                    const double score_text_duration = 2500;

                    this.TransformBindableTo(playerScoreValue, playerScore.TotalScore, score_text_duration, Easing.OutExpo);
                    this.TransformBindableTo(opponentScoreValue, opponentScore.TotalScore, score_text_duration, Easing.OutExpo);

                    playerScoreText.Delay(score_text_duration)
                                   .ScaleTo(1.1f, 200, Easing.Out)
                                   .Then()
                                   .ScaleTo(1f, 500, Easing.OutElasticHalf);

                    opponentScoreText.Delay(score_text_duration)
                                     .ScaleTo(1.1f, 200, Easing.Out)
                                     .Then()
                                     .ScaleTo(1f, 500, Easing.OutElasticHalf);

                    long maxScore = Math.Max(
                        Math.Max(playerScore.TotalScore, opponentScore.TotalScore),
                        1_000_000
                    );

                    float playerScorePercent = (float)playerScore.TotalScore / maxScore;
                    float opponentScorePercent = (float)opponentScore.TotalScore / maxScore;

                    playerScoreBar.FadeIn(100);
                    opponentScoreBar.FadeIn(100);

                    this.TransformBindableTo(scoreBarProgress, Math.Max(playerScorePercent, opponentScorePercent), score_text_duration, new CubicBezierEasingFunction(easeIn: 0.4, easeOut: 1));

                    scoreBarProgress.BindValueChanged(e =>
                    {
                        playerScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, playerScorePercent));
                        opponentScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, opponentScorePercent));
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

                delay += 1000;

                using (BeginDelayedSequence(delay))
                {
                    Schedule(() =>
                    {
                        foreach (var (userId, userInfo) in matchInfo.RoomState.Users)
                        {
                            if (userId == client.LocalUser!.UserID)
                                rankedPlayScreen.PlayerUserDisplay.Health.Value = userInfo.Life;
                            else
                                rankedPlayScreen.OpponentUserDisplay.Health.Value = userInfo.Life;
                        }
                    });
                }
            }

            public override void Hide()
            {
                cornerPieceVisibility.Value = Visibility.Hidden;

                content.MoveToY(400, 400, Easing.OutExpo)
                       .ScaleTo(0.8f, 400, Easing.OutExpo)
                       .FadeOut(200);
            }
        }
    }
}
