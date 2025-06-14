// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;

namespace osu.Game.Screens.Edit.Modding
{
    public partial class ModdingScreen : EditorScreenWithTimeline
    {
        public ModdingScreen()
            : base(EditorScreenMode.Modding)
        {
        }

        protected override Drawable CreateMainContent() => Empty();
    }
}
