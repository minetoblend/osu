// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.RankedPlay
{
    public interface IRankedPlayClient
    {
        Task RankedPlayCardRevealed(RankedPlayCard card, MultiplayerPlaylistItem item);
    }
}
