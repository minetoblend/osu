// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Online.Rooms;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public class RankedPlayCardWithPlaylistItem : IEquatable<RankedPlayCardWithPlaylistItem>
    {
        public readonly RankedPlayCardItem Card;

        /// <summary>
        /// Completes once the playlistItem for the given card is revealed to the local player.
        /// </summary>
        public Task<MultiplayerPlaylistItem> PlaylistItem => playlistItemTaskCompletionSource.Task;

        private readonly TaskCompletionSource<MultiplayerPlaylistItem> playlistItemTaskCompletionSource = new TaskCompletionSource<MultiplayerPlaylistItem>();

        public RankedPlayCardWithPlaylistItem(RankedPlayCardItem card)
        {
            Card = card;
        }

        public void PlaylistItemRevealed(MultiplayerPlaylistItem item)
        {
            playlistItemTaskCompletionSource.SetResult(item);
        }

        public bool Equals(RankedPlayCardWithPlaylistItem? other)
            => other != null && Card.Equals(other.Card);

        public override bool Equals(object? obj)
            => obj is RankedPlayCardWithPlaylistItem other && Equals(other);

        public override int GetHashCode()
            => Card.GetHashCode();
    }
}
