// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Edit.Tools
{
    public partial class ComposeToolbar : CompositeDrawable
    {
        public static Vector2 ButtonSize => new Vector2(48);

        private readonly ComposeToolInfo[] tools;

        private FillFlowContainer<ComposeToolButton> buttonsContainer = null!;

        public ComposeToolbar(IEnumerable<ComposeToolInfo> tools)
        {
            this.tools = tools.ToArray();
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;

            InternalChild = buttonsContainer = new FillFlowContainer<ComposeToolButton>
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(8),
                ChildrenEnumerable = tools.Select(tool => new ComposeToolButton(tool)),
            };
        }

        [Resolved]
        private HitObjectComposer composer { get; set; } = null!;

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (getButtonForKey(e.Key) is ComposeToolButton button)
            {
                composer.SetActiveTool(button.ToolInfo);
                return true;
            }

            return base.OnKeyDown(e);
        }

        private ComposeToolButton? getButtonForKey(Key key)
        {
            if (key >= Key.Number1 && key <= Key.Number9)
            {
                int index = key - Key.Number1;

                return buttonsContainer.Children.ElementAtOrDefault(index);
            }

            return null;
        }
    }
}
