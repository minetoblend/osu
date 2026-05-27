// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Edit.Tools
{
    public abstract class ComposeToolInfo
    {
        public readonly LocalisableString Name;

        protected ComposeToolInfo(LocalisableString name)
        {
            Name = name;
        }

        public virtual Drawable? CreateIcon() => null;

        public abstract ComposeTool CreateTool();
    }
}
