// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingScreen : EditorScreen
    {
        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }

        [Cached]
        private readonly Bindable<IReadOnlyList<ControlPoint>> selectedControlPoints = new Bindable<IReadOnlyList<ControlPoint>>(new List<ControlPoint>());

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

            selectionManager.SelectionChanged += _ => Scheduler.AddOnce(updateSelection);
        }

        private void updateSelection() => selectedControlPoints.Value = selectionManager.Selection.ToList();
    }
}
