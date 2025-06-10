// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Game.Rulesets.Osu.Skinning.Default;

namespace osu.Game.Rulesets.Osu.Skinning.Legacy
{
    public partial class LegacySliderSelect : ManualSliderBody
    {
        public LegacySliderSelect()
        {
            BorderColour = Color4Extensions.FromHex("#3197FF");
        }

        protected override DrawableSliderPath CreateSliderPath() => new LegacySliderBody.LegacyDrawableSliderPath();
    }
}
