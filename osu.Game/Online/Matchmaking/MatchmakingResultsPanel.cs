// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingResultsPanel : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private FillFlowContainer<MatchmakingFunStatistic> funStatistics = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = funStatistics = new FillFlowContainer<MatchmakingFunStatistic>
            {
                RelativeSizeAxes = Axes.Both
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

            funStatistics.Clear();

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

            for (int round = 1; round <= matchmakingState.Round; round++)
            {
                long roundHighestScore = long.MinValue;
                int roundHighestScoreUserId = 0;

                long roundLowestScore = long.MaxValue;

                foreach ((int userId, MatchmakingUser user) in matchmakingState.Users.UserDictionary)
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

            foreach ((int userId, MatchmakingUser user) in matchmakingState.Users.UserDictionary)
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
            addFunStatistic(maxScoreUserId, "Highest score");

            // Most accurate - highest accuracy across all rounds.
            addFunStatistic(maxAccuracyUserId, "Most accurate");

            // Most persistent - highest combo across all rounds.
            addFunStatistic(maxComboUserId, "Most persistent");

            // Hardest worker - most bonus score across all rounds.
            addFunStatistic(maxBonusScoreUserId, "Hardest worker");

            // Clutcher - smallest victory in any round.
            addFunStatistic(smallestScoreDifferenceUserId, "Clutcher");

            // Finisher - largest victory in any round.
            addFunStatistic(largestScoreDifferenceUserId, "Finisher");
        });

        private void addFunStatistic(int userId, string text)
        {
            MultiplayerRoomUser? user = client.Room?.Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null)
                return;

            funStatistics.Add(new MatchmakingFunStatistic(text, user));
        }
    }
}
