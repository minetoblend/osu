// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Rulesets.Edit.UI;

namespace osu.Game.Tests.Visual.Editing
{
    [TestFixture]
    public partial class TestSceneSidebar : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Aquamarine);

        private readonly ComposeSidebar sidebar;

        public TestSceneSidebar()
        {
            Add(new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = colourProvider.Background3,
                    },
                    sidebar = new ComposeSidebar
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                    }
                }
            });

            sidebar.Add(SidebarCategory.TOOLS, new SidebarPanel("Active Tool")
            {
                Child = new OsuSpriteText
                {
                    Text = "Tools",
                    Margin = new MarginPadding(10),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            });
            sidebar.Add(SidebarCategory.INSPECT, new SidebarPanel("Inspector")
            {
                Child = new OsuSpriteText
                {
                    Text = "Foo bar",
                    Margin = new MarginPadding(10),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            });
        }
    }
}
