// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingScoreChange
    {
        public int UserId { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
    }
}
