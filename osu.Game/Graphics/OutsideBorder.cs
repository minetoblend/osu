// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osuTK.Graphics;

namespace osu.Game.Graphics
{
    /// <summary>
    /// Drawable that draws a border on the outside of it's bounds rather than the inside
    /// </summary>
    public partial class OutsideBorder : CompositeDrawable
    {
        private float borderThickness;

        public new float BorderThickness
        {
            get => borderThickness;
            set
            {
                borderThickness = value;
                updateBorder();
            }
        }

        private float cornerRadius;

        public new float CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = value;
                updateBorder();
            }
        }

        private readonly Container borderContainer;

        public OutsideBorder()
        {
            InternalChild = borderContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                BorderColour = Color4.White,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true,
                }
            };
        }

        private void updateBorder()
        {
            Padding = new MarginPadding(-borderThickness + 1);
            borderContainer.BorderThickness = borderThickness;
            borderContainer.CornerRadius = cornerRadius + borderThickness;
        }
    }

    public static class BorderExtensions
    {
        public static TransformSequence<OutsideBorder> TransformBorderThicknessTo(this OutsideBorder drawable, float newValue, double duration = 0, Easing easing = Easing.None) =>
            drawable.TransformTo(nameof(OutsideBorder.BorderThickness), newValue, duration, easing);

        public static TransformSequence<OutsideBorder> TransformBorderThicknessTo(this TransformSequence<OutsideBorder> t, float newValue, double duration = 0, Easing easing = Easing.None) =>
            t.TransformTo(nameof(OutsideBorder.BorderThickness), newValue, duration, easing);
    }
}
