// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;

namespace osu.Game.Rulesets.Edit.Tools
{
    public abstract class SelectToolInfo : ComposeToolInfo
    {
        protected SelectToolInfo()
            : base("Select") { }

        public override Drawable CreateIcon() => new SpriteIcon { Icon = OsuIcon.EditorSelect };
    }
}
