// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Caching;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardHand
    {
        private readonly Cached<LayoutEntry[]> handLayoutBacking = new Cached<LayoutEntry[]>();
        private readonly Cached<LayoutEntry[]> lineupLayoutBacking = new Cached<LayoutEntry[]>();

        private LayoutEntry[] handLayout => handLayoutBacking.IsValid ? handLayoutBacking.Value : handLayoutBacking.Value = computeHandLayout().ToArray();
        private LayoutEntry[] lineupLayout => lineupLayoutBacking.IsValid ? lineupLayoutBacking.Value : lineupLayoutBacking.Value = computeLineupLayout().ToArray();

        private void updateLayout()
        {
            handLayoutBacking.Invalidate();
            lineupLayoutBacking.Invalidate();

            for (int i = 0; i < cards.Count; i++)
            {
                var cardState = cards[i].State;

                if (cardState == CardState.NewlyDrawn)
                    cardState = State;

                var entry = cardState switch
                {
                    CardState.Hand or CardState.NewlyDrawn => handLayout[i],
                    CardState.Hidden => handLayout[i].OffsetBy(new Vector2(0, 200)),
                    CardState.Lineup => lineupLayout[i],
                    _ => throw new ArgumentOutOfRangeException()
                };

                entry.Card.UpdateMovement(
                    entry.Position,
                    entry.Rotation
                );
            }
        }

        private IEnumerable<LayoutEntry> computeHandLayout()
        {
            if (cards.Count == 0)
                yield break;

            var hoveredCard = cards.FirstOrDefault(it => it.IsHovered);

            foreach ((var card, float x) in horizontalArrangement(spacing: -60))
            {
                var targetPosition = new Vector2(x + offsetToHoveredCard(card), 0);

                targetPosition.Y = MathF.Pow(MathF.Abs(targetPosition.X / 250), 2) * 20;

                float rotation = targetPosition.X * 0.03f;

                if (card.Selected)
                {
                    float angle = MathHelper.DegreesToRadians(rotation - 90);

                    targetPosition += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 80;
                }

                yield return new LayoutEntry(card, targetPosition, rotation);
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
            foreach ((var card, float x) in horizontalArrangement(spacing: 10))
                yield return new LayoutEntry(card, new Vector2(x, -100), 0);
        }

        private IEnumerable<(PlayerCard card, float x)> horizontalArrangement(float spacing) => horizontalArrangement(cards, spacing);

        private IEnumerable<(PlayerCard card, float x)> horizontalArrangement(IEnumerable<PlayerCard> cards, float spacing)
        {
            float totalWidth = cards.Sum(card => card.CardLayoutWidth + spacing) - spacing;
            float x = -totalWidth / 2;

            foreach (var card in cards)
            {
                yield return (card, x + card.CardLayoutWidth / 2);

                x += card.CardLayoutWidth + spacing;
            }
        }

        private readonly record struct LayoutEntry(PlayerCard Card, Vector2 Position, float Rotation)
        {
            public LayoutEntry OffsetBy(Vector2 offset) => this with { Position = Position + offset };
        }
    }
}
