// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Events;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public partial class OpponentHandFacadeContainer : PlayerHandFacadeContainer
    {
        private partial class OpponentCardFacade : HandCardFacade
        {
            public OpponentCardFacade(RankedPlayScreen.Card card)
                : base(card) { }

            public override bool OnCardHover(HoverEvent e) => false;

            public override bool OnCardMouseDown(MouseDownEvent e) => false;

            public override void OnCardMouseUp(MouseUpEvent e) { }

            public override bool OnCardClicked(ClickEvent e) => false;
        }
    }
}
