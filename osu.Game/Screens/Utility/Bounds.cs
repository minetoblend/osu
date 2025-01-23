// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Screens.Utility
{
    public class Bounds
    {
        private Matrix3 transform;

        public Bounds()
            : this(Matrix3.Identity)
        {
        }

        public Bounds(Matrix3 transform)
        {
            this.transform = transform;
        }

        public Bounds(Drawable drawable)
            : this(drawable.DrawInfo.MatrixInverse)
        {
        }

        public RectangleF? Rectangle { get; private set; }

        private Vector2 toLocalSpace(Vector2 position)
            => Vector2Extensions.Transform(position, transform);

        private Quad toLocalSpace(Quad quad) => quad * transform;

        public void Add(Vector2 screenSpacePosition, float radius = 0f)
        {
            var position = toLocalSpace(screenSpacePosition);

            var scale = transform.ExtractScale().Xy;

            var rect = new RectangleF(position, Vector2.Zero)
                .Inflate(radius * scale);

            if (Rectangle == null)
            {
                Rectangle = rect;
                return;
            }

            Rectangle = RectangleF.Union(Rectangle.Value, rect);
        }

        public void Add(Quad screenSpaceQuad)
        {
            var quad = toLocalSpace(screenSpaceQuad);

            if (Rectangle == null)
            {
                Rectangle = quad.AABBFloat;
                return;
            }

            Rectangle = RectangleF.Union(Rectangle.Value, quad.AABBFloat);
        }
    }
}
