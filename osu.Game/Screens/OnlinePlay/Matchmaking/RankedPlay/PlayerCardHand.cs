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
        private CardState state = CardState.Hidden;

        public CardState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                state = value;

                const double stagger = 50;
                double delay = 0;

                foreach (var card in cards)
                {
                    card.Delay(delay).ChangeStateTo(state);
                    delay += stagger;
                }
            }
        }

        public readonly BindableBool AllowSelection = new BindableBool();

        public IEnumerable<RankedPlayCardItem> Cards => cards.Select(it => it.Item);

        public override bool PropagatePositionalInputSubTree => State != CardState.Hidden;

        private readonly Container<PlayerCard> cardContainer;
        private readonly List<PlayerCard> cards = new List<PlayerCard>();

        public PlayerCardHand()
        {
            InternalChild = cardContainer = new Container<PlayerCard>
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void Clear()
        {
            cards.Clear();
            cardContainer.Clear();
        }

        public void AddCard(RankedPlayCardItem card, Action<PlayerCard>? setupAction = null)
        {
            var drawable = new PlayerCard(card)
            {
                AllowSelection = { BindTarget = AllowSelection },
                State = state,
                Y = DrawHeight / 2,
            };

            cards.Add(drawable);
            cardContainer.Add(drawable);

            setupAction?.Invoke(drawable);
        }

        public void DrawCard(RankedPlayCardItem card) => AddCard(card, d =>
        {
            d.State = CardState.NewlyDrawn;
            d.X = DrawWidth;
            d.Rotation = -30;
            d.FadeInFromZero(200);

            d.Delay(500).ChangeStateTo(state);
        });

        public void DiscardCards(RankedPlayCardItem[] items)
        {
            var drawables = cards.Where(card => items.Any(it => it.Equals(card.Item))).ToList();

            const double stagger = 50;
            double delay = 0;

            foreach ((var card, float x) in horizontalArrangement(drawables, spacing: 40))
            {
                cards.Remove(card);

                card
                    .Delay(delay)
                    .MoveTo(new Vector2(x, -130), 600, Easing.OutExpo)
                    .RotateTo(0, 600, Easing.OutExpo)
                    .Then(200)
                    .ScaleTo(0, 500, Easing.In)
                    .FadeOut(300)
                    .Expire();

                delay += stagger;
            }
        }

        public void DiscardSelectedCards() => DiscardCards(cards.Where(it => it.Selected).Select(it => it.Item).ToArray());

        protected override void Update()
        {
            base.Update();

            updateLayout();
        }

        public enum CardState
        {
            Hidden,
            Hand,
            Lineup,
            NewlyDrawn,
        }

        private static class CardMovement
        {
            public static readonly Spring ENERGETIC = new Spring
            {
                NaturalFrequency = 4,
                Damping = 0.8f,
                Response = 0f
            };

            public static readonly Spring SMOOTH = new Spring
            {
                NaturalFrequency = 5,
                Damping = 0.5f,
                Response = -3.25f
            };
        }
    }
}
