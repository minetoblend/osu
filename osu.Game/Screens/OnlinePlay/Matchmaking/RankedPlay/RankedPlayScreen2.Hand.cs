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

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen2
    {
        public partial class PlayerHand : CompositeDrawable, ICardFacadeContainer
        {
            private readonly Container<HandCardFacade> cardContainer;
            private readonly Dictionary<Card, HandCardFacade> facades = new Dictionary<Card, HandCardFacade>();

            private readonly BindableBool allowSelection = new BindableBool();

            private bool allowMultipleSelection;

            public void EnableSingleSelection()
            {
                ClearSelection();

                allowSelection.Value = true;
                allowMultipleSelection = false;
            }

            public void EnableMultiSelection()
            {
                allowSelection.Value = true;
                allowMultipleSelection = true;
            }

            public void DisableSelection()
            {
                allowSelection.Value = false;
            }

            public readonly HashSet<RankedPlayCardWithPlaylistItem> Selection = new HashSet<RankedPlayCardWithPlaylistItem>();

            public PlayerHand()
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

                if (!allowMultipleSelection && Selection.Count > 0)
                {
                    ClearSelection();
                }

                Selection.Add(card.Item);
                card.Selected = true;

                return true;
            }

            private void shakeCard(CardFacade card)
            {
                float angle = MathHelper.DegreesToRadians(card.Rotation);
                var offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

                card.MoveToOffset(offset * 20)
                    .Delay(100)
                    .MoveToOffset(offset * -30)
                    .Delay(100)
                    .MoveToOffset(offset * 20)
                    .Delay(100)
                    .MoveToOffset(offset * -10);
            }

            public CardFacade AddCard(Card card)
            {
                if (facades.TryGetValue(card, out var existing))
                    return existing;

                var facade = new HandCardFacade(card)
                {
                    InvalidateLayout = invalidateLayout,
                    Clicked = cardClicked,
                    Anchor = Anchor.BottomCentre,
                    AllowSelection = { BindTarget = allowSelection }
                };

                facades[card] = facade;

                cardContainer.Add(facade);

                invalidateLayout();

                return facade;
            }

            public void RemoveCard(Card card)
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
                    child.Y = MathF.Pow(MathF.Abs(x / 250), 2) * 20 - 100;
                    child.Rotation = x * 0.03f;

                    float yOffset = 0;

                    // if a card is hovered, we want to move the cards to it's right a bit further away so the card is fully visible
                    if (child.CardHovered)
                    {
                        x += 30;
                        xOffset = 30;
                        yOffset = 20;
                    }
                    else
                    {
                        x -= xOffset / 2;
                        xOffset /= 2;
                    }

                    float angle = MathHelper.DegreesToRadians(child.Rotation - 90);
                    child.Position += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * yOffset;

                    x += child.LayoutWidth / 2 + spacing;
                }
            }

            private void invalidateLayout() => Scheduler.AddOnce(updateLayout);

            private partial class HandCardFacade : CardFacade
            {
                private const float hovered_scale = 1.2f;

                public readonly Card Card;

                public readonly BindableBool AllowSelection = new BindableBool();

                public RankedPlayCardWithPlaylistItem Item => Card.Item;

                public float LayoutWidth => DrawWidth * (CardHovered ? hovered_scale : 1);

                public bool CardHovered { get; private set; }

                public required Action InvalidateLayout;

                public required Func<HandCardFacade, bool> Clicked;

                public HandCardFacade(Card card)
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
}
