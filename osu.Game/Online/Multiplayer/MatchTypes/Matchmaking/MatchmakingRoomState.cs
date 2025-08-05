// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    [MessagePackObject]
    public class MatchmakingRoomState : MatchRoomState
    {
        [Key(0)]
        public MatchmakingRoomStatus RoomStatus { get; set; }

        /// <summary>
        /// All playlist items that were picked as gameplay candidates.
        /// </summary>
        [Key(1)]
        public MultiplayerPlaylistItem[] CandidateItems { get; set; }

        /// <summary>
        /// The gameplay playlist item.
        /// </summary>
        [Key(2)]
        public MultiplayerPlaylistItem GameplayItem { get; set; }
    }
}
