// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    [Cached]
    public abstract partial class CardHand : CompositeDrawable
    {
        private const float hover_scale = 1.2f;

        public IEnumerable<HandCard> Cards => cardContainer.Children;

        /// <summary>
        /// How far a card slides upwards when hovered.
        /// Used for making sure a card moves entirely into frame when the hand is partially off-screen.
        /// </summary>
        public float HoverYOffset = 15;

        /// <summary>
        /// If true, card layout will be flipped on both axes for a card hand placed at the top edge of the screen, while keeping the cards upright.
        /// Used for <see cref="OpponentCardHand"/>.
        /// </summary>
        protected virtual bool Flipped => false;

        private readonly Container<HandCard> cardContainer;

        private readonly Dictionary<RankedPlayCardItem, HandCard> cardLookup = new Dictionary<RankedPlayCardItem, HandCard>();

        protected CardHand()
        {
            AddInternal(cardContainer = new Container<HandCard>
            {
                RelativeSizeAxes = Axes.Both,
            });
        }

        protected override void Update()
        {
            base.Update();

            if (!layoutBacking.IsValid)
            {
                updateLayout();
                layoutBacking.Validate();
            }
        }

        private bool contracted;

        /// <summary>
        /// Contracts all cards towards the bottom (or top when <see cref="Flipped"/>).
        /// Cards will no longer get layouted after this method is called.
        /// </summary>
        public void Contract()
        {
            contracted = true;

            double delay = 0;

            foreach (var card in cardContainer)
            {
                card.Delay(delay)
                    .MoveTo(new Vector2(0, Flipped ? -220 : 220), 400, Easing.OutExpo)
                    .RotateTo(0, 400, Easing.OutExpo)
                    .ScaleTo(1, 400, Easing.OutExpo);

                delay += 50;
            }
        }

        private Anchor cardAnchor => Flipped ? Anchor.TopCentre : Anchor.BottomCentre;

        public void AddCard(RankedPlayCardWithPlaylistItem item, Action<HandCard>? setupAction = null) => AddCard(new RankedPlayCard(item), setupAction);

        public void AddCard(RankedPlayCard card, Action<HandCard>? setupAction = null)
        {
            if (cardLookup.ContainsKey(card.Item.Card))
                return;

            var drawable = CreateCardFacade(card);
            drawable.Anchor = drawable.Origin = cardAnchor;

            cardLookup[card.Item.Card] = drawable;

            cardContainer.Add(drawable);
            layoutBacking.Invalidate();

            setupAction?.Invoke(drawable);
        }

        public bool RemoveCard(RankedPlayCardWithPlaylistItem item)
        {
            if (!cardLookup.Remove(item.Card, out var drawable))
                return false;

            cardContainer.Remove(drawable, true);
            layoutBacking.Invalidate();
            return false;
        }

        /// <summary>
        /// Removes a card and detaches it's contained card so it can be attached to a new card facade.
        /// </summary>
        /// <param name="item">Item to remove the card for</param>
        /// <param name="card">Contained <see cref="RankedPlayCard"/></param>
        /// <param name="screenSpaceDrawQuad"><see cref="Drawable.ScreenSpaceDrawQuad"/> of the removed card</param>
        /// <returns>Whether a card was found for the provided <see cref="item"/></returns>
        public bool RemoveCard(RankedPlayCardWithPlaylistItem item, [MaybeNullWhen(false)] out RankedPlayCard card, out Quad screenSpaceDrawQuad)
        {
            if (!cardLookup.Remove(item.Card, out var drawable))
            {
                card = null;
                screenSpaceDrawQuad = default;
                return false;
            }

            screenSpaceDrawQuad = drawable.ScreenSpaceDrawQuad;
            card = drawable.Detach();

            cardContainer.Remove(drawable, true);
            layoutBacking.Invalidate();

            return true;
        }

        protected virtual HandCard CreateCardFacade(RankedPlayCard card) => new HandCard(card);

        protected virtual void OnCardStateChanged(HandCard handCardFacade, CardState state) => InvalidateLayout();

        #region Layout

        private readonly Cached layoutBacking = new Cached();

        protected void InvalidateLayout() => layoutBacking.Invalidate();

        public void UpdateLayout(double stagger = 0)
        {
            updateLayout(stagger);
            layoutBacking.Validate();
        }

        private void updateLayout(double stagger = 0)
        {
            if (contracted)
                return;

            const float spacing = -40;

            float totalWidth = cardContainer.Sum(it => it.LayoutWidth + spacing) - spacing;

            float x = -totalWidth / 2;

            if (cardContainer.Any(it => it.CardHovered))
                x -= 20;

            float xOffset = 0;
            double delay = 0;

            foreach (var child in cardContainer)
            {
                x += child.LayoutWidth / 2;

                float yOffset = 0;

                var position = new Vector2(x, MathF.Pow(MathF.Abs(x / 250), 2) * 20 + 10);

                if (child.CardHovered)
                {
                    x += 30;
                    xOffset = 30;
                    yOffset = -HoverYOffset;
                }
                else
                {
                    x -= xOffset / 2;
                    xOffset /= 2;
                }

                float rotation = x * 0.03f;

                float angle = MathHelper.DegreesToRadians(rotation + 90);

                position += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * yOffset;

                position *= Flipped ? -1 : 1;

                child
                    .Delay(delay)
                    .MoveTo(position, 300, Easing.OutExpo)
                    .RotateTo(rotation, 300, Easing.OutExpo)
                    .ScaleTo(child.CardHovered ? hover_scale : 1f, 400, Easing.OutElasticQuarter);

                x += child.LayoutWidth / 2 + spacing;

                delay += stagger;
            }
        }

        #endregion

        public partial class HandCard : CompositeDrawable
        {
            public float LayoutWidth => DrawWidth * (State.Hovered ? hover_scale : 1);

            private CardState state;

            public CardState State
            {
                get => state;
                set
                {
                    if (state.Equals(value))
                        return;

                    state = value;

                    selectionOverlay.Alpha = state.Selected ? 1 : 0;

                    cardHand.OnCardStateChanged(this, value);
                }
            }

            public bool Selected
            {
                get => State.Selected;
                set => State = State with { Selected = value };
            }

            public bool CardHovered
            {
                get => State.Hovered;
                set => State = State with { Hovered = value };
            }

            private readonly Container selectionOverlay;

            [Resolved]
            private CardHand cardHand { get; set; } = null!;

            public readonly RankedPlayCard Card;

            public HandCard(RankedPlayCard card)
            {
                Size = card.DrawSize;

                card.Anchor = Anchor.Centre;
                card.Origin = Anchor.Centre;
                card.Position = Vector2.Zero;
                card.Rotation = 0;
                card.Scale = Vector2.One;

                AddInternal(Card = card);

                Anchor = Anchor.BottomCentre;
                Origin = Anchor.BottomCentre;

                card.OverlayLayer.Child = selectionOverlay = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 10,
                    BorderThickness = 4,
                    BorderColour = Color4Extensions.FromHex("72D5FF"),
                    Alpha = 0,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        AlwaysPresent = true
                    }
                };
            }

            public RankedPlayCard Detach()
            {
                Card.OverlayLayer.Clear();
                Card.Elevation = 0;

                RemoveInternal(Card, false);

                return Card;
            }

            protected override void Update()
            {
                base.Update();

                Card.Elevation = float.Lerp(CardHovered ? 1 : 0, Card.Elevation, (float)Math.Exp(-0.03f * Time.Elapsed));
            }
        }

        public readonly record struct CardState(bool Selected, bool Hovered);
    }
}
