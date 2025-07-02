// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Edit.UI;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Edit
{
    public partial class ActiveToolEditorToolboxGroup : EditorToolboxGroup
    {
        private readonly Bindable<PlacementBlueprint> activePlacement;

        private readonly ComposeBlueprintContainer blueprintContainer;

        public ActiveToolEditorToolboxGroup(ComposeBlueprintContainer blueprintContainer)
            : base("Active tool")
        {
            this.blueprintContainer = blueprintContainer;

            activePlacement = blueprintContainer.CurrentPlacementBindable.GetBoundCopy();
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activePlacement.BindValueChanged(e =>
            {
                var content = e.NewValue?.CreateSidebarContent();

                Title = $"Active Tool ({blueprintContainer.CurrentTool.Name})";

                if (content != null)
                {
                    Child = content;
                    Show();
                }
                else
                {
                    Clear();
                    Hide();
                }
            }, true);
        }
    }
}
