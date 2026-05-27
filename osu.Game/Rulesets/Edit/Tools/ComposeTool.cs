// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Edit.Tools
{
    public abstract partial class ComposeTool : CompositeDrawable
    {
        protected ComposeTool()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [Resolved]
        private HitObjectComposer composer { get; set; } = null!;

        [Resolved]
        private ComposeToolContainer composeToolContainer { get; set; } = null!;

        protected virtual void RecreateTool() => composeToolContainer.RecreateCurrentTool();

        public virtual void OnExit()
        {
        }
    }
}
