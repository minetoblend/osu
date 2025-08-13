// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Rulesets.Scoring;
using osu.Game.Utils;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingResultsPanel : CompositeDrawable
    {
        private const float grid_spacing = 5;

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private OsuSpriteText placementText = null!;
        private FillFlowContainer<MatchmakingBreakdownStatistic> userStatistics = null!;
        private FillFlowContainer<MatchmakingRoomStatistic> roomStatistics = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions =
                [
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension(GridSizeMode.Absolute, grid_spacing),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, grid_spacing),
                    new Dimension(GridSizeMode.AutoSize)
                ],
                Content = new Drawable[]?[]
                {
                    [
                        placementText = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Font = OsuFont.Default.With(size: 72)
                        }
                    ],
                    null,
                    [
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions =
                            [
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.Absolute, grid_spacing),
                                new Dimension()
                            ],
                            Content = new Drawable?[][]
                            {
                                [
                                    new FillFlowContainer
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Children = new Drawable[]
                                        {
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                Text = "Breakdown"
                                            },
                                            userStatistics = new FillFlowContainer<MatchmakingBreakdownStatistic>
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                            }
                                        }
                                    },
                                    null,
                                    new MatchmakingPlayerList
                                    {
                                        RelativeSizeAxes = Axes.Both
                                    }
                                ]
                            }
                        }
                    ],
                    null,
                    [
                        roomStatistics = new FillFlowContainer<MatchmakingRoomStatistic>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y
                        }
                    ]
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onRoomStateChanged;
            onRoomStateChanged(client.Room?.MatchState);
        }

        private void onRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState || matchmakingState.RoomStatus != MatchmakingRoomStatus.RoomEnd)
                return;

            placementText.Text = $"#{matchmakingState.Users[client.LocalUser!.UserID].Placement}";

            populateUserStatistics(matchmakingState);
            populateRoomStatistics(matchmakingState);
        });

        private void populateUserStatistics(MatchmakingRoomState state)
        {
            userStatistics.Clear();

            int overallPlacement = state.Users[client.LocalUser!.UserID].Placement;
            int overallPoints = state.Users[client.LocalUser!.UserID].Points;
            int bestPlacement = state.Users[client.LocalUser!.UserID].Rounds.Min(r => r.Placement);
            var accuracyPlacement = state.Users.Select(u => (user: u, avgAcc: u.Rounds.Average(r => r.Accuracy)))
                                         .OrderByDescending(t => t.avgAcc)
                                         .Select((t, i) => (info: t, index: i))
                                         .Single(t => t.info.user.UserId == client.LocalUser!.UserID);

            addStatistic($"#{overallPlacement} overall ({overallPoints}pts)");
            addStatistic($"#{bestPlacement} best placement");
            addStatistic($"#{accuracyPlacement.index + 1} accuracy ({accuracyPlacement.info.avgAcc.FormatAccuracy()})");

            void addStatistic(string text)
            {
                userStatistics.Add(new MatchmakingBreakdownStatistic(text)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                });
            }
        }

        private void populateRoomStatistics(MatchmakingRoomState state)
        {
            roomStatistics.Clear();

            long maxScore = long.MinValue;
            int maxScoreUserId = 0;

            double maxAccuracy = double.MinValue;
            int maxAccuracyUserId = 0;

            int maxCombo = int.MinValue;
            int maxComboUserId = 0;

            long maxBonusScore = 0;
            int maxBonusScoreUserId = 0;

            long largestScoreDifference = long.MinValue;
            int largestScoreDifferenceUserId = 0;

            long smallestScoreDifference = long.MaxValue;
            int smallestScoreDifferenceUserId = 0;

            for (int round = 1; round <= state.Round; round++)
            {
                long roundHighestScore = long.MinValue;
                int roundHighestScoreUserId = 0;

                long roundLowestScore = long.MaxValue;

                foreach ((int userId, MatchmakingUser user) in state.Users.UserDictionary)
                {
                    if (!user.Rounds.RoundsDictionary.TryGetValue(round, out MatchmakingRound? mmRound))
                        continue;

                    if (mmRound.TotalScore > maxScore)
                    {
                        maxScore = mmRound.TotalScore;
                        maxScoreUserId = userId;
                    }

                    if (mmRound.Accuracy > maxAccuracy)
                    {
                        maxAccuracy = mmRound.Accuracy;
                        maxAccuracyUserId = userId;
                    }

                    if (mmRound.MaxCombo > maxCombo)
                    {
                        maxCombo = mmRound.MaxCombo;
                        maxComboUserId = userId;
                    }

                    if (mmRound.TotalScore > roundHighestScore)
                    {
                        roundHighestScore = mmRound.TotalScore;
                        roundHighestScoreUserId = userId;
                    }

                    if (mmRound.TotalScore < roundLowestScore)
                        roundLowestScore = mmRound.TotalScore;
                }

                long roundScoreDifference = roundHighestScore - roundLowestScore;

                if (roundScoreDifference > 0 && roundScoreDifference > largestScoreDifference)
                {
                    largestScoreDifference = roundScoreDifference;
                    largestScoreDifferenceUserId = roundHighestScoreUserId;
                }

                if (roundScoreDifference > 0 && roundScoreDifference < smallestScoreDifference)
                {
                    smallestScoreDifference = roundScoreDifference;
                    smallestScoreDifferenceUserId = roundHighestScoreUserId;
                }
            }

            foreach ((int userId, MatchmakingUser user) in state.Users.UserDictionary)
            {
                int userBonusScore = 0;

                foreach ((_, MatchmakingRound round) in user.Rounds.RoundsDictionary)
                {
                    userBonusScore += round.Statistics.TryGetValue(HitResult.LargeBonus, out int bonus) ? bonus * 5 : 0;
                    userBonusScore += round.Statistics.TryGetValue(HitResult.SmallBonus, out bonus) ? bonus : 0;
                }

                if (userBonusScore > maxBonusScore)
                {
                    maxBonusScore = userBonusScore;
                    maxBonusScoreUserId = userId;
                }
            }

            // Highest score - highest score across all rounds.
            addStatistic(maxScoreUserId, "Highest score");

            // Most accurate - highest accuracy across all rounds.
            addStatistic(maxAccuracyUserId, "Most accurate");

            // Most persistent - highest combo across all rounds.
            addStatistic(maxComboUserId, "Most persistent");

            // Hardest worker - most bonus score across all rounds.
            if (maxBonusScoreUserId > 0)
                addStatistic(maxBonusScoreUserId, "Hardest worker");

            // Clutcher - smallest victory in any round.
            if (smallestScoreDifferenceUserId > 0)
                addStatistic(smallestScoreDifferenceUserId, "Clutcher");

            // Finisher - largest victory in any round.
            if (largestScoreDifferenceUserId > 0)
                addStatistic(largestScoreDifferenceUserId, "Finisher");

            void addStatistic(int userId, string text)
            {
                MultiplayerRoomUser? user = client.Room?.Users.FirstOrDefault(u => u.UserID == userId);

                if (user == null)
                    throw new InvalidOperationException($"User not found in room: {userId}");

                roomStatistics.Add(new MatchmakingRoomStatistic(text, user)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                });
            }
        }
    }
}
