// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    [MessagePackObject]
    public class MatchmakingUserScoreList
    {
        [Key(0)]
        public IDictionary<int, MatchmakingUserScore> Scores { get; set; } = new Dictionary<int, MatchmakingUserScore>();

        public void AddPoints(int userId, int placement, int points)
        {
            if (!Scores.TryGetValue(userId, out MatchmakingUserScore? userScore))
                Scores[userId] = userScore = new MatchmakingUserScore(userId);

            userScore.RoundPlacements.Add(placement);
            userScore.Points += points;
        }

        public void AdjustPlacements()
        {
            int i = 1;
            foreach (var score in Scores.Values.Order(new MatchmakingUserScoreComparer()))
                score.Placement = i++;
        }
    }
}
