// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;

namespace osu.Game.Screens.Edit.HitSounds
{
    public partial class HitSoundScreen : EditorScreenWithTimeline
    {
        public HitSoundScreen()
            : base(EditorScreenMode.HitSounds)
        {
        }

        protected override Drawable CreateMainContent() => new ScreenWhiteBox.UnderConstructionMessage("HitSounding mode");
    }
}
