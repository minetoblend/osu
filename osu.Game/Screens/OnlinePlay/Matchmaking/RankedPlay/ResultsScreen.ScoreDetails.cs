// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        private partial class ScoreDetails(RankedPlayColourScheme colours, ScoreInfo score) : CompositeDrawable
        {
            private ScoreStatisticsDisplay scoreStatisticsDisplay = null!;
            private ScoreRankDisplay scoreRankDisplay = null!;
            private FillFlowContainer accuracyDisplay = null!;
            private FillFlowContainer comboDisplay = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                Padding = new MarginPadding(20) { Horizontal = 40 };

                InternalChild = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(30),
                    Children =
                    [
                        scoreStatisticsDisplay = new ScoreStatisticsDisplay(score, colours)
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            ColumnDimensions =
                            [
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(),
                            ],
                            RowDimensions = [new Dimension(GridSizeMode.AutoSize)],
                            Content = new Drawable[][]
                            {
                                [
                                    scoreRankDisplay = new ScoreRankDisplay(score)
                                    {
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.BottomCentre,
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Spacing = new Vector2(20),
                                        Children =
                                        [
                                            accuracyDisplay = new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                                Children =
                                                [
                                                    new OsuSpriteText
                                                    {
                                                        Text = "Accuracy",
                                                        UseFullGlyphHeight = false,
                                                    },
                                                    new OsuSpriteText
                                                    {
                                                        Text = score.DisplayAccuracy,
                                                        Font = OsuFont.GetFont(size: 36, weight: FontWeight.SemiBold)
                                                    },
                                                ]
                                            },
                                            comboDisplay = new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                                Children =
                                                [
                                                    new OsuSpriteText
                                                    {
                                                        Text = "Combo",
                                                        UseFullGlyphHeight = false,
                                                    },
                                                    new OsuSpriteText
                                                    {
                                                        Text = $"{score.MaxCombo}x",
                                                        Font = OsuFont.GetFont(size: 36, weight: FontWeight.SemiBold)
                                                    },
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        }
                    ]
                };
            }

            public void Appear(double duration)
            {
                this.FadeIn(200);
            }
        }
    }
}
