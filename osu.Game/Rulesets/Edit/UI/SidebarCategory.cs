// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Rulesets.Edit.UI
{
    public record SidebarCategory(LocalisableString Name, int Priority)
    {
        public static readonly SidebarCategory TOOLS = new SidebarCategory("Tools", 0);

        public static readonly SidebarCategory SNAPPING = new SidebarCategory("Grid & Snapping", 1);

        public static readonly SidebarCategory INSPECT = new SidebarCategory("Inspector", 2);
    }
}
