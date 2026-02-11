// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Logging;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Models;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;
using ScoreCounter = osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components.ScoreCounter;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen : RankedPlaySubScreen
    {
        public override bool ShowBeatmapBackground => true;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        [Resolved]
        private ScoreManager scoreManager { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        [Resolved]
        private IBindable<RulesetInfo> globalRuleset { get; set; } = null!;

        private LoadingSpinner loadingSpinner = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            CornerPieceVisibility.Value = Visibility.Hidden;

            InternalChildren = new Drawable[]
            {
                loadingSpinner = new LoadingSpinner
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            loadingSpinner.Show();

            queryScores().FireAndForget();
        }

        private async Task queryScores()
        {
            try
            {
                if (client.Room == null)
                    return;

                Task<APIBeatmap?> beatmapTask = beatmapLookupCache.GetBeatmapAsync(client.Room.CurrentPlaylistItem.BeatmapID);
                TaskCompletionSource<List<MultiplayerScore>> scoreTask = new TaskCompletionSource<List<MultiplayerScore>>();

                var request = new IndexPlaylistScoresRequest(client.Room.RoomID, client.Room.Settings.PlaylistItemId);
                request.Success += req => scoreTask.SetResult(req.Scores);
                request.Failure += scoreTask.SetException;
                api.Queue(request);

                await Task.WhenAll(beatmapTask, scoreTask.Task).ConfigureAwait(false);

                APIBeatmap? apiBeatmap = beatmapTask.GetResultSafely();
                List<MultiplayerScore> apiScores = scoreTask.Task.GetResultSafely();

                if (apiBeatmap == null)
                    return;

                // Reference: PlaylistItemResultsScreen
                setScores(apiScores.Select(s => s.CreateScoreInfo(scoreManager, rulesets, new BeatmapInfo
                {
                    Difficulty = new BeatmapDifficulty(apiBeatmap.Difficulty),
                    Metadata =
                    {
                        Artist = apiBeatmap.Metadata.Artist,
                        Title = apiBeatmap.Metadata.Title,
                        Author = new RealmUser
                        {
                            Username = apiBeatmap.Metadata.Author.Username,
                            OnlineID = apiBeatmap.Metadata.Author.OnlineID,
                        }
                    },
                    DifficultyName = apiBeatmap.DifficultyName,
                    StarRating = apiBeatmap.StarRating,
                    Length = apiBeatmap.Length,
                    BPM = apiBeatmap.BPM
                })).ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load scores for playlist item.");
                throw;
            }
            finally
            {
                Scheduler.Add(() => loadingSpinner.Hide());
            }
        }

        private ScreenScaffold? scaffold;

        [Resolved]
        private RankedPlayMatchInfo matchInfo { get; set; } = null!;

        private static Vector2 cardSize => new Vector2(950, 600);

        private readonly Bindable<Visibility> cornerPieceVisibility = new Bindable<Visibility>();
        private readonly Bindable<float> scoreBarProgress = new Bindable<float>();

        private void setScores(ScoreInfo[] scores) => Scheduler.Add(() =>
        {
            int playerId = client.LocalUser!.UserID;
            int opponentId = matchInfo.RoomState.Users.Keys.Single(u => u != playerId);

            ScoreInfo playerScore = scores.SingleOrDefault(s => s.UserID == api.LocalUser.Value.OnlineID) ?? new ScoreInfo
            {
                Rank = ScoreRank.F,
                Ruleset = globalRuleset.Value
            };

            ScoreInfo opponentScore = scores.SingleOrDefault(s => s.UserID != api.LocalUser.Value.OnlineID) ?? new ScoreInfo
            {
                Rank = ScoreRank.F,
                Ruleset = globalRuleset.Value
            };

            Box flash;
            ScoreDetails playerScoreDetails;
            ScoreDetails opponentScoreDetails;
            ScoreCounter playerScoreCounter;
            ScoreCounter opponentScoreCounter;
            ScoreCounter damageCounter;
            ScoreBar playerScoreBar;
            ScoreBar opponentScoreBar;
            OsuSpriteText roundNumber;

            AddInternal(scaffold = new ScreenScaffold
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children =
                [
                    new RankedPlayCornerPiece(RankedPlayColourScheme.Blue, Anchor.BottomLeft)
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        State = { BindTarget = cornerPieceVisibility },
                        Child = new RankedPlayUserDisplay(playerId, Anchor.BottomLeft, RankedPlayColourScheme.Blue)
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    },
                    new RankedPlayCornerPiece(RankedPlayColourScheme.Red, Anchor.BottomRight)
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        State = { BindTarget = cornerPieceVisibility },
                        Child = new RankedPlayUserDisplay(opponentId, Anchor.BottomRight, RankedPlayColourScheme.Red)
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 110,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Padding = new MarginPadding { Bottom = 30 },
                        Child = roundNumber = new OsuSpriteText
                        {
                            Text = $"Round {matchInfo.CurrentRound}",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.GetFont(size: 36, weight: FontWeight.Bold, typeface: Typeface.TorusAlternate),
                            Alpha = 0,
                        },
                    },
                    new GridContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = cardSize,
                        Padding = new MarginPadding { Bottom = 110, Top = 60, Horizontal = 60 },
                        ColumnDimensions =
                        [
                            new Dimension(),
                            new Dimension(GridSizeMode.Absolute, 40),
                            new Dimension(GridSizeMode.Absolute, 60),
                            new Dimension(GridSizeMode.Absolute, 10),
                            new Dimension(GridSizeMode.Absolute, 60),
                            new Dimension(GridSizeMode.Absolute, 40),
                            new Dimension(),
                        ],
                        Content = new Drawable?[][]
                        {
                            [
                                new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    RowDimensions =
                                    [
                                        new Dimension(),
                                        new Dimension(GridSizeMode.AutoSize)
                                    ],
                                    Content = new Drawable[][]
                                    {
                                        [
                                            playerScoreDetails = new ScoreDetails(playerScore, RankedPlayColourScheme.Blue)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Alpha = 0,
                                            },
                                        ],
                                        [
                                            playerScoreCounter = new ScoreCounter
                                            {
                                                Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(-4),
                                                Alpha = 0,
                                                AlwaysPresent = true,
                                            }
                                        ]
                                    }
                                },
                                null,
                                playerScoreBar = new ScoreBar(RankedPlayColourScheme.Blue)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Height = 0,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,
                                    Alpha = 0,
                                },
                                null,
                                opponentScoreBar = new ScoreBar(RankedPlayColourScheme.Red)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Height = 0,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,
                                    Alpha = 0,
                                },
                                null,
                                new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    RowDimensions =
                                    [
                                        new Dimension(),
                                        new Dimension(GridSizeMode.AutoSize)
                                    ],
                                    Content = new Drawable[][]
                                    {
                                        [
                                            opponentScoreDetails = new ScoreDetails(opponentScore, RankedPlayColourScheme.Red)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Alpha = 0,
                                            },
                                        ],
                                        [
                                            opponentScoreCounter = new ScoreCounter
                                            {
                                                Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(-4),
                                                Alpha = 0,
                                                AlwaysPresent = true,
                                            }
                                        ]
                                    }
                                },
                            ]
                        }
                    },
                    flash = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                ],
                BottomOrnament =
                {
                    Size = new Vector2(200, 60),
                    Alpha = 0,
                    Children =
                    [
                        damageCounter = new ScoreCounter(7)
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Font = OsuFont.GetFont(size: 36, weight: FontWeight.SemiBold, fixedWidth: true),
                            Spacing = new Vector2(-2),
                        }
                    ]
                }
            });

            double delay = 0;

            scaffold.FadeIn(100)
                    .ResizeTo(0)
                    .ResizeTo(cardSize with { Y = 30 }, 600, Easing.OutExpo)
                    // deliberately cutting this delay 300ms short so the vertical resize interrupts the horizontal one
                    .Delay(300)
                    .ResizeHeightTo(cardSize.Y, 800, Easing.OutExpo);

            flash.Delay(150)
                 .FadeOut(600, Easing.Out);

            Scheduler.AddDelayed(() => cornerPieceVisibility.Value = Visibility.Visible, 700);

            scaffold.BottomOrnament
                    .Delay(900)
                    .FadeIn(300)
                    .ResizeWidthTo(cardSize.X - 550, 600, Easing.OutExpo);

            roundNumber.Delay(700).FadeIn(600);
            playerScoreCounter.Delay(700).FadeIn(600);
            opponentScoreCounter.Delay(700).FadeIn(600);

            delay += 1000;

            using (BeginDelayedSequence(delay))
            {
                const double score_text_duration = 3000;

                playerScoreCounter.TransformValueTo(playerScore.TotalScore, score_text_duration - 500);
                opponentScoreCounter.TransformValueTo(opponentScore.TotalScore, score_text_duration - 500);

                damageCounter.TransformValueTo(123456 /* TODO */, score_text_duration - 500);

                long maxAchievableScore = Math.Max(
                    Math.Max(playerScore.TotalScore, opponentScore.TotalScore),
                    1_000_000
                );

                float playerScorePercent = (float)playerScore.TotalScore / maxAchievableScore;
                float opponentScorePercent = (float)opponentScore.TotalScore / maxAchievableScore;
                float maxScorePercent = Math.Max(playerScorePercent, opponentScorePercent);

                playerScoreBar.FadeIn(100);
                opponentScoreBar.FadeIn(100);

                this.TransformBindableTo(scoreBarProgress, maxScorePercent, score_text_duration, new CubicBezierEasingFunction(easeIn: 0.4, easeOut: 1));

                scoreBarProgress.BindValueChanged(e =>
                {
                    playerScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, playerScorePercent));
                    opponentScoreBar.Height = float.Lerp(0.05f, 1f, Math.Min(e.NewValue, opponentScorePercent));
                });
            }

            delay += 3500;

            using (BeginDelayedSequence(delay))
            {
                playerScoreDetails.FadeIn(300);
                opponentScoreDetails.FadeIn(300);
            }
        });
    }
}
