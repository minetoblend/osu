// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Screens.Edit
{
    public partial class Editor
    {
        public const float BUTTON_HEIGHT = 40;
        public const float BUTTON_CORNER_RADIUS = 5;
        public const float BUTTON_ICON_SIZE = 18;

        public static class Fonts
        {
            public static FontUsage Default => OsuFont.Style.Body;

            public static FontUsage Heading => OsuFont.Style.Heading2.With(weight: FontWeight.Bold);
        }
    }
}
