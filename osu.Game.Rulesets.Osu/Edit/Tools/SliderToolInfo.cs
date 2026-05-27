// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Rulesets.Edit.Tools;

namespace osu.Game.Rulesets.Osu.Edit.Tools
{
    public class SliderToolInfo : ComposeToolInfo
    {
        public SliderToolInfo()
            : base("Slider")
        {
        }

        public override Drawable? CreateIcon() => new SpriteIcon { Icon = OsuIcon.EditorSlider };

        public override ComposeTool CreateTool() => new SliderTool();
    }
}
