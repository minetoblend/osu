// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using MessagePack;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    /// <summary>
    /// Describes a user of a matchmaking room.
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class MatchmakingUser
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        [Key(0)]
        public required int UserId { get; set; }

        /// <summary>
        /// The aggregate room placement (1-based).
        /// </summary>
        [Key(1)]
        public int? Placement { get; set; }

        /// <summary>
        /// The aggregate points.
        /// </summary>
        [Key(2)]
        public int Points { get; set; }

        /// <summary>
        /// The scores set.
        /// </summary>
        [Key(3)]
        public MatchmakingRoundList Rounds { get; set; } = new MatchmakingRoundList();

        /// <summary>
        /// Represents the set of cards in this user's current hand.
        /// Any of these are playable.
        /// </summary>
        [Key(5)]
        public MultiplayerPlaylistItem[] Hand { get; set; } = [];

        /// <summary>
        /// The current life points.
        /// </summary>
        [Key(6)]
        public int Life { get; set; } = 1_000_000;
    }
}
