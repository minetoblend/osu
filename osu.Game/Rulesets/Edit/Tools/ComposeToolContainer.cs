// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Edit.Tools
{
    [Cached]
    public class ComposeToolContainer : CompositeDrawable
    {
        [Resolved]
        private IBindable<ComposeToolInfo> toolInfo { get; set; } = null!;

        private ComposeTool? currentTool;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            toolInfo.BindValueChanged(_ => recreateTool(), true);
        }

        private void recreateTool()
        {
            currentTool?.OnExit();
            currentTool?.Expire();
            AddInternal(currentTool = toolInfo.Value.CreateTool());
        }

        public void RecreateCurrentTool() => Scheduler.AddOnce(recreateTool);
    }
}
