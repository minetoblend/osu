// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using MessagePack;

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    [MessagePackObject]
    public class MatchmakingUserScore
    {
        // The user.
        [Key(0)]
        public readonly int UserId;

        /// <summary>
        /// Aggregate room placement.
        /// </summary>
        [Key(1)]
        public int Placement { get; set; }

        /// <summary>
        /// Current total points.
        /// </summary>
        [Key(2)]
        public int Points { get; set; }

        /// <summary>
        /// Individual round placements.
        /// </summary>
        [Key(3)]
        public IList<int> RoundPlacements { get; set; } = new List<int>();

        [SerializationConstructor]
        public MatchmakingUserScore(int userId)
        {
            UserId = userId;
        }
    }
}
