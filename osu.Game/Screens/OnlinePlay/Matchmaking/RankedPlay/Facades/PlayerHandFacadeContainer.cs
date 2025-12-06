// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public partial class PlayerHandFacadeContainer : CompositeDrawable, ICardFacadeContainer
    {
        private readonly Container<HandCardFacade> cardContainer;
        private readonly Dictionary<RankedPlayScreen.Card, HandCardFacade> facades = new Dictionary<RankedPlayScreen.Card, HandCardFacade>();

        private readonly BindableBool allowSelection = new BindableBool();

        private float contractedAmount;

        public float ContractedAmount
        {
            get => contractedAmount;
            set
            {
                contractedAmount = float.Clamp(value, 0, 1);
                invalidateLayout();
            }
        }

        private CardSelectionMode selectionMode;

        public CardSelectionMode SelectionMode
        {
            get => selectionMode;
            set
            {
                selectionMode = value;
                allowSelection.Value = value != CardSelectionMode.Disabled;
            }
        }

        public readonly HashSet<RankedPlayCardWithPlaylistItem> Selection = new HashSet<RankedPlayCardWithPlaylistItem>();

        public PlayerHandFacadeContainer()
        {
            InternalChild = cardContainer = new Container<HandCardFacade>
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void ClearSelection()
        {
            Selection.Clear();

            foreach (var card in cardContainer)
                card.Selected = false;
        }

        private bool cardClicked(HandCardFacade card)
        {
            if (!allowSelection.Value)
                return false;

            if (Selection.Contains(card.Item))
            {
                Selection.Remove(card.Item);
                card.Selected = false;
                return true;
            }

            if (selectionMode == CardSelectionMode.Single && Selection.Count > 0)
            {
                ClearSelection();
            }

            Selection.Add(card.Item);
            card.Selected = true;

            return true;
        }

        public CardFacade AddCard(RankedPlayScreen.Card card)
        {
            if (facades.TryGetValue(card, out var existing))
                return existing;

            var facade = new HandCardFacade(card)
            {
                InvalidateLayout = invalidateLayout,
                Clicked = cardClicked,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                CardMovementBindable = { Value = RankedPlayScreen.MovementStyle.Energetic },
                AllowSelection = { BindTarget = allowSelection },
            };

            facades[card] = facade;

            cardContainer.Add(facade);

            invalidateLayout();

            return facade;
        }

        public void RemoveCard(RankedPlayScreen.Card card)
        {
            if (!facades.Remove(card, out var facade))
                return;

            cardContainer.Remove(facade, true);
            Selection.Remove(card.Item);

            invalidateLayout();
        }

        private void updateLayout()
        {
            const float spacing = -20;

            float totalWidth = cardContainer.Sum(it => it.LayoutWidth + spacing) - spacing;
            float x = -totalWidth / 2;

            float xOffset = 0;

            if (cardContainer.Any(it => it.IsHovered))
                x -= 20;

            foreach (var child in cardContainer)
            {
                x += child.LayoutWidth / 2;

                child.X = x;
                child.Y = MathF.Pow(MathF.Abs(x / 250), 2) * 20;
                child.Rotation = x * 0.03f;

                // if a card is hovered, we want to move the cards to it's right a bit further away so the card is fully visible
                if (child.CardHovered)
                {
                    x += 30;
                    xOffset = 30;
                }
                else
                {
                    x -= xOffset / 2;
                    xOffset /= 2;
                }

                child.Position = Vector2.Lerp(child.Position, new Vector2(child.X / 2, 150), contractedAmount);

                x += child.LayoutWidth / 2 + spacing;
            }
        }

        private void invalidateLayout() => Scheduler.AddOnce(updateLayout);

        private partial class HandCardFacade : CardFacade
        {
            private const float hovered_scale = 1.2f;

            public readonly RankedPlayScreen.Card Card;

            public readonly BindableBool AllowSelection = new BindableBool();

            public RankedPlayCardWithPlaylistItem Item => Card.Item;

            public float LayoutWidth => DrawWidth * (CardHovered ? hovered_scale : 1);

            public bool CardHovered { get; private set; }

            public required Action InvalidateLayout;

            public required Func<HandCardFacade, bool> Clicked;

            public HandCardFacade(RankedPlayScreen.Card card)
            {
                Card = card;
            }

            public override bool OnCardHover(HoverEvent e)
            {
                CardHovered = true;
                updateState();
                return true;
            }

            public override void OnCardHoverLost(HoverLostEvent e)
            {
                CardHovered = false;
                updateState();
            }

            public override bool OnCardMouseDown(MouseDownEvent e)
            {
                if (e.Button == MouseButton.Left && AllowSelection.Value)
                {
                    CardPressed = true;
                    updateState();
                    return true;
                }

                return base.OnCardMouseDown(e);
            }

            public override void OnCardMouseUp(MouseUpEvent e)
            {
                if (e.Button == MouseButton.Left)
                {
                    CardPressed = false;
                    updateState();
                }
            }

            public override bool OnCardClicked(ClickEvent e)
            {
                if (!AllowSelection.Value)
                    return false;

                return Clicked(this);
            }

            public bool CardPressed;

            private void updateState()
            {
                Scale = new Vector2(CardHovered ? hovered_scale : 1);
                if (CardPressed)
                    Scale *= 0.97f;

                DebugOverlay.BorderColour = (CardPressed, CardHovered) switch
                {
                    (true, _) => Color4.Yellow,
                    (_, true) => Color4.Green,
                    _ => Color4.Red
                };

                InvalidateLayout();
            }
        }
    }
}
