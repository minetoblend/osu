// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingScreen : EditorScreen
    {
        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }

        private DependencyContainer dependencies = null!;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            return dependencies = new DependencyContainer(parent);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            ControlPointSelectionManager selectionManager;

            AddInternal(selectionManager = new ControlPointSelectionManager());

            dependencies.CacheAs(selectionManager);

            Add(new LayeredTimeline
            {
                Children = new TimelineLayer[]
                {
                    new TimingPointLayer()
                }
            });
        }
    }
}
