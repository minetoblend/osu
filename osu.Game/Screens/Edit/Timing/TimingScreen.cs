// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingScreen : EditorScreen
    {
        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }

        [Cached]
        private readonly ControlPointSelectionManager selectionManager = new ControlPointSelectionManager();

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(selectionManager);

            Add(new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 350),
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new LayeredTimeline
                        {
                            Children = new TimelineLayer[]
                            {
                                new TimingPointLayer()
                            }
                        },
                        new ControlPointSettings(),
                    },
                }
            });
        }
    }
}
