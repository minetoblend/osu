// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen
    {
        private readonly Dictionary<RankedPlayCardItem, Card> cardLookup = new Dictionary<RankedPlayCardItem, Card>();
        private readonly Dictionary<Card, ICardFacadeContainer> activeFacade = new Dictionary<Card, ICardFacadeContainer>();

        private Card? cardForItem(RankedPlayCardItem item) => cardLookup.GetValueOrDefault(item);
        private Card? cardForItem(RankedPlayCardWithPlaylistItem item) => cardLookup.GetValueOrDefault(item.Card);

        private CardFacade addCard(RankedPlayCardWithPlaylistItem card, ICardFacadeContainer facadeContainer, Action<Card>? setupAction = null)
        {
            var drawable = cardLookup[card.Card] = new Card(card);
            var facade = facadeContainer.AddCard(drawable);

            drawable.ChangeFacade(facade);
            activeFacade[drawable] = facadeContainer;

            cardContainer.Add(drawable);

            setupAction?.Invoke(drawable);

            return facade;
        }

        private void removeCard(RankedPlayCardWithPlaylistItem card)
        {
            var drawable = cardContainer.FirstOrDefault(it => it.Item.Equals(card));

            if (drawable != null)
            {
                activeFacade.Remove(drawable, out var facadeContainer);

                cardContainer.Remove(drawable, true);
                facadeContainer?.RemoveCard(drawable);
            }
        }

        private bool moveCardToContainer(RankedPlayCardWithPlaylistItem item, ICardFacadeContainer facadeContainer, double moveDelay = 0, double removeFromPreviousLayoutDelay = 0)
        {
            if (cardForItem(item) is not { } card)
                return false;

            moveCardToContainer(card, facadeContainer, moveDelay, removeFromPreviousLayoutDelay);
            return true;
        }

        private void moveCardToContainer(Card card, ICardFacadeContainer facadeContainer, double moveDelay = 0, double removeFromPreviousLayoutDelay = 0)
        {
            removeFromPreviousLayoutDelay += moveDelay;

            if (activeFacade.TryGetValue(card, out var previousContainer))
            {
                if (previousContainer == facadeContainer)
                    return;

                if (removeFromPreviousLayoutDelay <= 0)
                    previousContainer.RemoveCard(card);
                else
                {
                    Scheduler.AddDelayed(() =>
                    {
                        if (activeFacade.GetValueOrDefault(card) != previousContainer)
                            previousContainer.RemoveCard(card);
                    }, removeFromPreviousLayoutDelay);
                }
            }

            activeFacade[card] = facadeContainer;

            var facade = facadeContainer.AddCard(card);

            card.ChangeFacade(facade, moveDelay);
        }

        private void addCardExpiryListener()
        {
            cardContainer.CardRemoved += card =>
            {
                cardLookup.Remove(card.Item.Card);
                activeFacade.Remove(card, out var facadeContainer);
                facadeContainer?.RemoveCard(card);
            };
        }

        public interface ICardFacadeContainer
        {
            CardFacade AddCard(Card card);

            void RemoveCard(Card card);
        }

        public partial class CardFacade : CompositeDrawable
        {
            public bool Selected;

            [Resolved(name: "debugEnabled")]
            private Bindable<bool>? debugEnabled { get; set; }

            public readonly Bindable<SpringParameters> CardMovement = new Bindable<SpringParameters>(MovementStyle.Smooth);

            protected readonly Container DebugOverlay;

            public CardFacade()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Size = new Vector2(120, 200);
                Padding = new MarginPadding(-10);

                InternalChild = DebugOverlay = new Container
                {
                    RelativeSizeAxes = Axes.Both,

                    Masking = true,
                    MaskingSmoothness = 1,

                    BorderColour = Color4.Red.Opacity(0.3f),
                    BorderThickness = 1.5f,
                    Padding = new MarginPadding(-1),
                    Alpha = 0,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.05f,
                    },
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                debugEnabled?.BindValueChanged(e => DebugOverlay.Alpha = e.NewValue ? 1 : 0, true);
            }

            public virtual bool OnCardHover(HoverEvent e) => false;

            public virtual void OnCardHoverLost(HoverLostEvent e) { }

            public virtual bool OnCardClicked(ClickEvent e) => false;

            public virtual bool OnCardMouseDown(MouseDownEvent e) => false;

            public virtual void OnCardMouseUp(MouseUpEvent e) { }

            public virtual bool OnCardDragStart(DragStartEvent e) => false;

            public virtual void OnCardDrag(DragEvent e) { }

            public virtual void OnCardDragEnd(DragEndEvent e) { }
        }

        public partial class CardFillFlowContainer : FillFlowContainer<CardFacade>, ICardFacadeContainer
        {
            private readonly Dictionary<Card, CardFacade> facades = new Dictionary<Card, CardFacade>();

            public readonly Bindable<SpringParameters> CardMovement = new Bindable<SpringParameters>(MovementStyle.Energetic);

            public CardFacade AddCard(Card card)
            {
                if (facades.TryGetValue(card, out var existing))
                    return existing;

                var facade = new CardFacade
                {
                    CardMovement = { BindTarget = CardMovement }
                };

                Add(facade);
                facades[card] = facade;

                return facade;
            }

            public void RemoveCard(Card card)
            {
                if (facades.Remove(card, out var facade))
                    Remove(facade, true);
            }
        }

        private partial class CardContainer : Container<Card>
        {
            public event Action<Card>? CardRemoved;

            protected override bool RemoveInternal(Drawable drawable, bool disposeImmediately)
            {
                if (!base.RemoveInternal(drawable, disposeImmediately))
                    return false;

                if (drawable is Card card)
                    CardRemoved?.Invoke(card);
                return true;
            }
        }
    }
}
