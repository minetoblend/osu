// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingInQueueStatus : MatchmakingQueueStatus
    {
        /// <summary>
        /// The number of players found.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// The total room size.
        /// </summary>
        public int RoomSize { get; set; }
    }
}
