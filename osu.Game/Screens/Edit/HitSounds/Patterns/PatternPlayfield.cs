// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class PatternPlayfield : ScrollingPlayfield
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            RegisterPool<HitSound, DrawableHitSound>(20, 50);
        }
    }
}
