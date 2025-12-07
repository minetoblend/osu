// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PickScreen : RankedPlaySubScreen
    {
        private PlayerHandFacadeContainer playerHand = null!;
        private PlayerHandFacadeContainer contractedPlayerHand = null!;
        private ShearedButton playButton = null!;
        private CardFacade playedCardFacade = null!;
        private FillFlowContainer textContainer;

        public override double CardTransitionStagger => 50;

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
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
                contractedPlayerHand = new PlayerHandFacadeContainer
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.5f),
                    ContractedAmount = 1f,
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
                textContainer = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children =
                    [
                        new OsuSpriteText
                        {
                            Text = "First pick!",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Font = OsuFont.Style.Title,
                            Margin = new MarginPadding(20)
                        },
                        new OsuSpriteText
                        {
                            Text = "It’s your turn!",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Colour = colour.Blue,
                            Font = OsuFont.Style.Subtitle,
                        },
                    ]
                }
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

            Scheduler.AddDelayed(() =>
            {
                textContainer.FadeOut(50);
                double delay = 0;

                foreach (var c in playerHand.Cards)
                {
                    var facade = contractedPlayerHand.AddCard(c.Card);

                    facade.CardMovement = RankedPlayScreen.MovementStyle.Slow;
                    c.Card.ChangeFacade(facade, delay);

                    delay += 25;
                }
            }, 0);
        }
    }
}
