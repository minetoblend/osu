// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Utils;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardHand : CompositeDrawable
    {
        private readonly Container<PlayerCard> cardContainer;

        private readonly List<PlayerCard> cards = new List<PlayerCard>();

        private CardState state = CardState.Hand;
        private double stateTransitionTime;

        public IEnumerable<RankedPlayCard> Cards => cards.Select(it => it.Item);

        public readonly BindableBool AllowSelection = new BindableBool();

        public CardState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                state = value;
                stateTransitionTime = Time.Current;
            }
        }

        public PlayerCardHand()
        {
            Size = new Vector2(750, 300);

            InternalChild = cardContainer = new Container<PlayerCard>
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void AddCard(RankedPlayCard card, Action<PlayerCard>? setupAction = null)
        {
            var drawable = new PlayerCard(card)
            {
                AllowSelection = { BindTarget = AllowSelection },
            };

            cards.Add(drawable);
            cardContainer.Add(drawable);

            setupAction?.Invoke(drawable);
        }

        public void DrawCard(RankedPlayCard card) => AddCard(card, d =>
        {
            d.NewlyAdded = true;
            d.X = DrawWidth;
            d.Rotation = -30;
            d.FadeInFromZero(200);

            Scheduler.AddDelayed(() => d.NewlyAdded = false, 500);
        });

        public void Discard(RankedPlayCard[] items)
        {
            var drawables = cards.Where(card => items.Any(it => it.Equals(card.Item))).ToList();

            float totalWidth = drawables.Skip(1).Sum(it => it.DrawWidth + 40);
            float x = -totalWidth / 2;

            const double stagger = 50;
            double delay = 0;

            foreach (var card in drawables)
            {
                cards.Remove(card);

                card
                    .Delay(delay)
                    .MoveTo(new Vector2(x, -400), 600, Easing.OutExpo)
                    .RotateTo(0, 600, Easing.OutExpo)
                    .Then(200)
                    .ScaleTo(0, 400, Easing.In)
                    .FadeOut(200)
                    .Expire();

                x += card.DrawWidth + 40;

                delay += stagger;
            }
        }

        public void DiscardSelectedCards() => Discard(cards.Where(it => it.Selected).Select(it => it.Item).ToArray());

        protected override void Update()
        {
            base.Update();

            updateLayout();
        }

        private void updateLayout()
        {
            foreach (var entry in computeCardLayout())
            {
                entry.Card.UpdateMovement(
                    entry.movement,
                    entry.Position,
                    entry.Rotation
                );
            }
        }

        private IEnumerable<LayoutEntry> computeCardLayout()
        {
            const float stagger = 30;

            int staggerCount = int.Clamp((int)((Time.Current - stateTransitionTime) / stagger), 0, cards.Count);

            return State switch
            {
                CardState.Hand =>
                [
                    ..computeHandLayout().Take(staggerCount),
                    ..computeLineupLayout().Skip(staggerCount)
                ],
                CardState.Lineup =>
                [
                    ..computeLineupLayout().Take(staggerCount),
                    ..computeHandLayout().Skip(staggerCount)
                ],
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private IEnumerable<LayoutEntry> computeHandLayout()
        {
            if (cards.Count == 0)
                yield break;

            const float spacing = -60;

            float totalWidth = cards.Skip(1).Sum(card => cardWidth(card) + spacing);
            float x = -totalWidth / 2;

            var hoveredCard = cards.FirstOrDefault(it => it.IsHovered);

            foreach (var card in cards)
            {
                var targetPosition = new Vector2(x + offsetToHoveredCard(card), 0);

                targetPosition.Y = MathF.Pow(MathF.Abs(targetPosition.X / 250), 2) * 20;

                float rotation = targetPosition.X * 0.03f;

                if (card.Selected)
                {
                    float angle = MathHelper.DegreesToRadians(rotation - 90);

                    targetPosition += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 80;
                }

                yield return new LayoutEntry(card, targetPosition, rotation, card.NewlyAdded ? CardMovement.DAMPED : CardMovement.DEFAULT);

                x += cardWidth(card) + spacing;
            }

            float offsetToHoveredCard(PlayerCard card)
            {
                if (hoveredCard is not null && card.X > hoveredCard.X)
                    return 5000 / (card.X - hoveredCard.X);

                return 0;
            }
        }

        private IEnumerable<LayoutEntry> computeLineupLayout()
        {
            const float spacing = 10;

            float totalWidth = cards.Skip(1).Sum(card => cardWidth(card) + spacing);
            float x = -totalWidth / 2;

            foreach (var card in cards)
            {
                yield return new LayoutEntry(card, new Vector2(x, -100), 0, CardMovement.DAMPED);

                x += cardWidth(card) + spacing;
            }
        }

        private float cardWidth(PlayerCard card) => card.LayoutSize.X * card.Scale.X;

        private readonly record struct LayoutEntry(PlayerCard Card, Vector2 Position, float Rotation, Spring movement);

        public enum CardState
        {
            Hand,
            Lineup,
        }
    }
}
