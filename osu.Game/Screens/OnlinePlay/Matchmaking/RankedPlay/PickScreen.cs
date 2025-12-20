// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Linq;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PickScreen : RankedPlaySubScreen
    {
        private PlayerHandFacadeContainer playerHand = null!;
        private CardFacade hiddenPlayerFacade = null!;
        private OpponentHandFacadeContainer opponentHand = null!;
        private CardFacade hiddenOpponentFacade = null!;
        private ShearedButton playButton = null!;
        private CardFacade playedCardFacade = null!;
        private FillFlowContainer textContainer = null!;

        public override double CardTransitionStagger => 50;

        [BackgroundDependencyLoader]
        private void load()
        {
            var matchState = Client.Room?.MatchState as RankedPlayRoomState;

            Debug.Assert(matchState != null);

            CenterColumn.Children =
            [
                playerHand = new PlayerHandFacadeContainer
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f,
                    SelectionMode = CardSelectionMode.Single,
                },
                opponentHand = new OpponentHandFacadeContainer
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    RelativeSizeAxes = Axes.Both,
                    Rotation = 180,
                    Height = 0.5f,
                    SelectionMode = CardSelectionMode.Disabled,
                    Y = -20,
                    ContractedAmount = 0.5f,
                },
                playedCardFacade = new CardFacade
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(1.2f)
                },
                hiddenPlayerFacade = new CardFacade
                {
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.TopCentre,
                    Y = 30,
                    CardMovement = RankedPlayScreen.MovementStyle.Slow,
                },
                hiddenOpponentFacade = new CardFacade
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.BottomCentre,
                    Y = -30,
                    CardMovement = RankedPlayScreen.MovementStyle.Slow,
                },
                textContainer = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(20),
                    Children =
                    [
                        new OsuSpriteText
                        {
                            Text = $"{FormatRoundIndex(matchState.CurrentRound).Titleize()} pick!",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Font = OsuFont.GetFont(typeface: Typeface.TorusAlternate, size: 42, weight: FontWeight.Regular),
                        },
                        new OsuSpriteText
                        {
                            Text = "It’s your turn!",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Colour = Color4Extensions.FromHex("87CDFF"),
                            Font = OsuFont.GetFont(typeface: Typeface.TorusAlternate, size: 28, weight: FontWeight.SemiBold),
                        },
                    ]
                },
            ];

            ButtonsContainer.Child = playButton = new ShearedButton(width: 150)
            {
                Action = onPlayButtonClicked,
                Enabled = { Value = true },
                Text = "Play",
            };
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

        public override ICardFacadeContainer OpponentCardContainer => opponentHand;

        public override void CardPlayed(RankedPlayScreen.Card card)
        {
            playerHand.RemoveCard(card);
            card.ChangeFacade(playedCardFacade);

            textContainer.FadeOut(50);

            card.ChangeFacade(playedCardFacade);

            double delay = 0;

            foreach (var c in playerHand.Cards)
            {
                if (c.Card == card)
                    continue;

                c.Card.ChangeFacade(hiddenPlayerFacade, delay);

                delay += 25;
            }

            delay = opponentHand.Cards.Count() * 25;

            foreach (var c in opponentHand.Cards)
            {
                if (c.Card == card)
                    continue;

                c.Card.ChangeFacade(hiddenOpponentFacade, delay);

                delay -= 25;
            }
        }
    }
}
