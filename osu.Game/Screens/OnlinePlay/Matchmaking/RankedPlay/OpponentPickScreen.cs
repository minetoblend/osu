// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class OpponentPickScreen : RankedPlaySubScreen
    {
        private PlayerHandFacadeContainer playerHand = null!;
        private CardFacade playedCardFacade = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                playerHand = new PlayerHandFacadeContainer
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.5f),
                    SelectionMode = CardSelectionMode.Disabled,
                    ContractedAmount = 0.5f,
                },
                playedCardFacade = new CardFacade
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
            ];
        }

        public override ICardFacadeContainer PlayerCardContainer => playerHand;

        public override void CardPlayed(RankedPlayScreen.Card card)
        {
            card.Position = new Vector2(DrawWidth / 2, 0);

            card.ChangeFacade(playedCardFacade);
        }
    }
}
