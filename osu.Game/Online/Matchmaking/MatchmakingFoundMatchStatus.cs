// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingFoundMatchStatus : MatchmakingQueueStatus
    {
        /// <summary>
        /// The ID of the room to join.
        /// </summary>
        public long RoomId { get; set; }
    }
}
