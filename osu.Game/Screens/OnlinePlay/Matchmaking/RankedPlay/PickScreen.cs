// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PickScreen : RankedPlaySubScreen
    {
        private PlayerHandFacadeContainer playerHand = null!;
        private ShearedButton playButton = null!;
        private CardFacade playedCardFacade = null!;

        public override double CardTransitionStagger => 50;

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
                    SelectionMode = CardSelectionMode.Single,
                },
                playedCardFacade = new CardFacade
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                playButton = new ShearedButton(width: 150)
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -100,
                    Action = onPlayButtonClicked,
                    Enabled = { Value = true },
                    Text = "Play",
                },
            ];
        }

        private void onPlayButtonClicked()
        {
            if (playerHand.Selection.Count > 0)
            {
                playerHand.SelectionMode = CardSelectionMode.Disabled;
                Client.PlayCard(playerHand.Selection.First().Card).FireAndForget();
                playButton.Hide();
            }
        }

        public override ICardFacadeContainer PlayerCardContainer => playerHand;

        public override void CardPlayed(RankedPlayScreen.Card card)
        {
            playerHand.RemoveCard(card);

            card.ChangeFacade(playedCardFacade);
        }
    }
}
