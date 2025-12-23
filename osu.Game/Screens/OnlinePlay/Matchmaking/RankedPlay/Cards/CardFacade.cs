// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Layout;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    /// <summary>
    /// Utility drawable which allows seamlessly moving cards between different containers.
    /// </summary>
    public partial class CardFacade : CompositeDrawable
    {
        public readonly RankedPlayCard Card;

        public CardFacade(RankedPlayCard card)
        {
            Size = card.DrawSize;

            card.Anchor = Anchor.Centre;
            card.Origin = Anchor.Centre;

            AddInternal(Card = card);
        }

        /// <summary>
        /// Removes the card from this facade without disposing it so it can be inserted into another container.
        /// </summary>
        public virtual RankedPlayCard Detach()
        {
            RemoveInternal(Card, false);

            return Card;
        }

        private Quad? pendingScreenSpaceDrawQuadUpdate;

        /// <summary>
        /// When written to, will update its transforms to match the provided drawQuad.
        /// </summary>
        /// <remarks>
        /// If the drawable has no parent, the transform update will be delayed until the drawable becomes alive.
        /// </remarks>
        public new Quad ScreenSpaceDrawQuad
        {
            get => base.ScreenSpaceDrawQuad;
            set
            {
                if (Parent == null)
                {
                    pendingScreenSpaceDrawQuadUpdate = value;
                    return;
                }

                pendingScreenSpaceDrawQuadUpdate = null;

                var drawQuad = Parent.ToLocalSpace(value);

                var originPosition = RelativeOriginPosition;

                Position = Vector2.Lerp(
                    Vector2.Lerp(drawQuad.TopLeft, drawQuad.TopRight, originPosition.X),
                    Vector2.Lerp(drawQuad.BottomLeft, drawQuad.BottomRight, originPosition.X),
                    originPosition.Y
                ) - AnchorPosition;

                Rotation = MathHelper.RadiansToDegrees(new Line(drawQuad.TopLeft, drawQuad.TopRight).Theta);

                Scale = new Vector2(Vector2.Distance(drawQuad.TopLeft, drawQuad.TopRight) / DrawWidth);
            }
        }

        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            if ((invalidation & Invalidation.Parent) != 0 && Parent != null && pendingScreenSpaceDrawQuadUpdate.HasValue)
            {
                ScreenSpaceDrawQuad = pendingScreenSpaceDrawQuadUpdate.Value;
            }

            return base.OnInvalidate(invalidation, source);
        }

        public void PopOutAndExpire()
        {
            Card.FadeOut(300)
                .ScaleTo(0, 500, Easing.In)
                .Then()
                .Expire();
        }

        public override double LifetimeEnd => Math.Min(base.LifetimeEnd, Card.LifetimeEnd);
    }
}
