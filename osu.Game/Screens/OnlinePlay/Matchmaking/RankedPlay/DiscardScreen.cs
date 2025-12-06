// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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

        public override void CardAdded(RankedPlayScreen.Card card)
        {
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

        public override void CardRemoved(RankedPlayScreen.Card card)
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
    }
}
