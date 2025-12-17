// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public partial class CardFillFlowContainer : FillFlowContainer<CardFacade>, ICardFacadeContainer
    {
        private readonly Dictionary<RankedPlayScreen.Card, CardFacade> facades = new Dictionary<RankedPlayScreen.Card, CardFacade>();

        public readonly Bindable<SpringParameters> CardMovement = new Bindable<SpringParameters>(RankedPlayScreen.MovementStyle.Energetic);

        public CardFacade AddCard(RankedPlayScreen.Card card)
        {
            if (facades.TryGetValue(card, out var existing))
                return existing;

            var facade = new CardFacade
            {
                CardMovementBindable = { BindTarget = CardMovement }
            };

            Add(facade);
            facades[card] = facade;

            return facade;
        }

        public void RemoveCard(RankedPlayScreen.Card card)
        {
            if (facades.Remove(card, out var facade))
                Remove(facade, true);
        }
    }
}
