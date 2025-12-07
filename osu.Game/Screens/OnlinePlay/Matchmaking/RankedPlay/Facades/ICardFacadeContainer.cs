// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public interface ICardFacadeContainer
    {
        CardFacade AddCard(RankedPlayScreen.Card card);

        void RemoveCard(RankedPlayScreen.Card card);
    }
}
