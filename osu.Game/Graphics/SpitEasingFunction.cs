// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;

namespace osu.Game.Graphics
{
    public readonly struct SplitEasingFunction(DefaultEasingFunction easeIn, DefaultEasingFunction easeOut, float ratio) : IEasingFunction
    {
        public SplitEasingFunction(Easing easeIn, Easing easeOut, float ratio = 0.5f)
            : this(new DefaultEasingFunction(easeIn), new DefaultEasingFunction(easeOut), ratio)
        {
        }

        public double ApplyEasing(double time)
        {
            if (time < ratio)
                return easeIn.ApplyEasing(time / ratio) * ratio;

            return double.Lerp(ratio, 1, easeOut.ApplyEasing((time - ratio) / (1 - ratio)));
        }
    }
}
