// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public class CardRow : Container<CardFacade>
    {
        public float Spacing = 20;

        public void LayoutCards(double stagger = 0, double duration = 400, Easing easing = Easing.OutExpo)
        {
            // makes sure that all facades had a chance to initialize their transforms based on the provided drawQuad
            CheckChildrenLife();

            float totalWidth = Children.Sum(c => c.LayoutSize.X + Spacing) - Spacing;

            float x = -totalWidth / 2;

            double delay = 0;

            foreach (var card in Children)
            {
                card.Delay(delay)
                    .MoveTo(new Vector2(x + card.LayoutSize.X * 0.5f, 0), duration, easing)
                    .RotateTo(0, duration, easing)
                    .ScaleTo(1, duration, easing);

                x += card.LayoutSize.X + Spacing;

                delay += stagger;
            }
        }

        public bool RemoveCard(RankedPlayCardWithPlaylistItem item, [MaybeNullWhen(false)] out RankedPlayCard card, out Quad screenSpaceDrawQuad)
        {
            var facade = Children.FirstOrDefault(it => it.Card.Item.Equals(item));

            if (facade == null)
            {
                card = null;
                screenSpaceDrawQuad = default;
                return false;
            }

            screenSpaceDrawQuad = facade.ScreenSpaceDrawQuad;
            card = facade.Detach();

            Remove(facade, true);

            return true;
        }
    }
}
