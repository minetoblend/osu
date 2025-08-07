// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    [MessagePackObject]
    public class MatchmakingRoomState : MatchRoomState
    {
        [Key(0)]
        public MatchmakingRoomStatus RoomStatus { get; set; }

        /// <summary>
        /// The playlist items that were picked as gameplay candidates.
        /// </summary>
        [Key(1)]
        public long[] CandidateItems { get; set; } = [];

        [Key(2)]
        public long CandidateItem { get; set; }

        [Key(3)]
        public MatchmakingUserScoreList UserScores { get; set; } = new MatchmakingUserScoreList();
    }
}
