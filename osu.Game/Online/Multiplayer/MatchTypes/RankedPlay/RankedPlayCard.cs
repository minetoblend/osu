// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.Multiplayer.MatchTypes.RankedPlay
{
    [Serializable]
    [MessagePackObject]
    public class RankedPlayCard : IEquatable<RankedPlayCard>
    {
        /// <summary>
        /// A unique identifier for this card.
        /// </summary>
        [Key(0)]
        public Guid ID { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The playlist item which this card corresponds to.
        /// </summary>
        /// <remarks>
        /// Not serialised - revealed to clients via <see cref="IMatchmakingClient.RankedPlayCardRevealed"/>.
        /// </remarks>
        [IgnoreMember]
        public MultiplayerPlaylistItem? Item { get; set; }

        public bool Equals(RankedPlayCard? other)
            => other != null && ID.Equals(other.ID);

        public override bool Equals(object? obj)
            => obj is RankedPlayCard other && Equals(other);

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
