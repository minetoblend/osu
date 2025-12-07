// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public abstract partial class RankedPlaySubScreen : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected MultiplayerClient Client => client;

        public virtual double CardTransitionStagger => 0;

        protected RankedPlaySubScreen()
        {
            RelativeSizeAxes = Axes.Both;
        }

        public virtual void OnEntering(RankedPlaySubScreen? previous)
        {
        }

        public virtual void OnExiting(RankedPlaySubScreen? next)
        {
            Hide();
        }

        public virtual void CardAdded(RankedPlayScreen.Card card, CardOwner owner)
        {
        }

        public virtual void CardRemoved(RankedPlayScreen.Card card, CardOwner owner)
        {
        }

        public virtual void CardPlayed(RankedPlayScreen.Card card)
        {
        }

        public virtual ICardFacadeContainer? PlayerCardContainer => null;

        public virtual ICardFacadeContainer? OpponentCardContainer => null;
    }
}
