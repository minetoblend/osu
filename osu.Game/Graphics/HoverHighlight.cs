// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace osu.Game.Graphics
{
    public partial class HoverHighlight : CompositeDrawable
    {
        public float HoverAlpha { get; set; } = 0.1f;

        public float HoverFlashAlpha { get; set; } = 0.25f;

        public double HoverFlashFadeInDuration { get; set; } = 50;

        public double HoverFlashFadeOutDuration { get; set; } = 400;

        public double FadeOutDuration { get; set; } = 300;

        public HoverHighlight()
        {
            RelativeSizeAxes = Axes.Both;
            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            };
        }

        protected override bool OnHover(HoverEvent e)
        {
            ClearTransforms(true);
            InternalChild
                .FadeTo(HoverFlashAlpha, HoverFlashFadeInDuration, Easing.Out)
                .Then()
                .FadeTo(HoverAlpha, HoverFlashFadeOutDuration, Easing.Out);

            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            ClearTransforms(true);

            InternalChild
                .FadeOut(FadeOutDuration, Easing.Out);
        }
    }
}
