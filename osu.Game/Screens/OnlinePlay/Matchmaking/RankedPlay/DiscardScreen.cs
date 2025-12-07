// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class DiscardScreen : RankedPlaySubScreen
    {
        private PlayerHandFacadeContainer playerHand = null!;
        private CardFillFlowContainer centerRow = null!;

        public override double CardTransitionStagger => 50;

        private CardFacade insertionFacade = null!;
        private ShearedButton discardButton = null!;
        private OsuSpriteText readyToGo = null!;
        private OsuTextFlowContainer explainer = null!;

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
                    SelectionMode = CardSelectionMode.Multiple,
                },
                centerRow = new CardFillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(20),
                    CardMovement = { Value = RankedPlayScreen.MovementStyle.Smooth },
                },
                insertionFacade = new CardFacade
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.Centre,
                    Y = -120,
                    Rotation = -30,
                },
                discardButton = new ShearedButton(width: 150)
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = -100,
                    Action = onDiscardButtonClicked,
                    Enabled = { Value = true },
                    Text = "Discard",
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Children =
                    [
                        new OsuSpriteText
                        {
                            Text = "Discarding Phase",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Font = OsuFont.Style.Title,
                            Margin = new MarginPadding(20)
                        },
                        readyToGo = new OsuSpriteText
                        {
                            Text = "You’re ready to go!",
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Colour = colour.Blue,
                            Font = OsuFont.Style.Subtitle,
                            Alpha = 0,
                        },
                        explainer = new OsuTextFlowContainer(s => s.Font = OsuFont.Style.Heading2)
                        {
                            AutoSizeAxes = Axes.Y,
                            Width = 500,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            TextAnchor = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 50 },
                            ParagraphSpacing = 1,
                            Alpha = 0,
                        }.With(d =>
                        {
                            d.AddParagraph("These are your Cards for this match!");
                            d.AddParagraph("When it’s your pick, you can choose one card to go head-to-head with against your opponent!");
                        })
                    ]
                }
            ];
        }

        public override ICardFacadeContainer PlayerCardContainer => playerHand;

        private double lastCardInsertionTime = double.MinValue;
        private double lastDiscardTime = double.MinValue;

        private void onDiscardButtonClicked()
        {
            discardButton.Hide();
            playerHand.SelectionMode = CardSelectionMode.Disabled;

            Client.DiscardCards(playerHand.Selection.Select(it => it.Card).ToArray()).FireAndForget();
        }

        public override void CardAdded(RankedPlayScreen.Card card, CardOwner owner)
        {
            if (owner == CardOwner.Opponent)
                return;

            card.Position = new Vector2(DrawWidth, DrawHeight - 120);
            card.Rotation = -30;

            card.ChangeFacade(insertionFacade);

            double insertionTime = Math.Max(Time.Current, lastCardInsertionTime + 100);
            lastCardInsertionTime = insertionTime;

            double delay = insertionTime - Time.Current;

            card.FadeOut()
                .Delay(delay)
                .FadeIn();

            Scheduler.AddDelayed(() =>
            {
                var facade = playerHand.AddCard(card);

                card.ChangeFacade(facade);

                facade.CardMovementBindable.Value = RankedPlayScreen.MovementStyle.Smooth;

                Scheduler.AddDelayed(() => facade.CardMovementBindable.Value = RankedPlayScreen.MovementStyle.Energetic, 300);
            }, delay);
        }

        public override void CardRemoved(RankedPlayScreen.Card card, CardOwner owner)
        {
            double discardTime = Math.Max(Time.Current, lastDiscardTime + 50);
            lastDiscardTime = discardTime;

            lastCardInsertionTime = discardTime + 1500;

            double delay = discardTime - Time.Current;

            card.ChangeFacade(centerRow.AddCard(card), delay);

            Scheduler.AddDelayed(() =>
            {
                playerHand.RemoveCard(card);

                card.PopOutAndExpire(1000);
            }, delay);
        }

        public void PresentRemainingCards(RankedPlayScreen.Card[] cards)
        {
            centerRow.Clear();

            const double stagger = 50;
            double delay = 0;

            centerRow.CardMovement.Value = RankedPlayScreen.MovementStyle.Slow;

            foreach (var card in cards)
            {
                var facade = centerRow.AddCard(card);

                card.ChangeFacade(facade, delay);
                delay += stagger;
            }

            readyToGo.FadeIn(50);
            explainer.FadeIn(50);
        }
    }
}
