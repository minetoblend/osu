// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardDeck : CompositeDrawable
    {
        private readonly Container<PlayerCard> cardContainer;

        public PlayerCardDeck()
        {
            Size = new Vector2(750, 300);

            InternalChild = cardContainer = new Container<PlayerCard>
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        public void AddCard(RankedPlayCard card)
        {
            var drawable = new PlayerCard(card);

            cardContainer.Add(drawable);
        }

        private InputManager inputManager = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            inputManager = GetContainingInputManager()!;
        }

        protected override void Update()
        {
            base.Update();

            const float spacing = -40;

            float totalWidth = cardContainer.Sum(card => card.DrawWidth * card.Scale.X + spacing) - spacing;
            float x = -totalWidth / 2;

            var hoveredCard = inputManager.HoveredDrawables.FirstOrDefault(it => it is PlayerCard);

            foreach (var card in cardContainer)
            {
                float cardWidth = card.DrawWidth * card.Scale.X;

                x += cardWidth / 2;

                var targetPosition = new Vector2(x, 0);

                if (hoveredCard is not null && card != hoveredCard)
                {
                    float direction = MathF.Sign(hoveredCard.X - card.X);
                    float distance = MathF.Abs(hoveredCard.X - card.X);

                    if (direction < 0)
                        targetPosition.X -= direction * 5000 / distance;
                }

                targetPosition.Y = MathF.Pow(MathF.Abs(targetPosition.X / 250), 2) * 20;

                card.TargetPosition = targetPosition;
                card.TargetRotation = targetPosition.X * 0.03f;

                x += cardWidth / 2 + spacing;
            }
        }
    }
}
