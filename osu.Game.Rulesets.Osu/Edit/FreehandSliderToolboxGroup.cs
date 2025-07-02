// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Edit.UI;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class FreehandSliderToolboxGroup : CompositeDrawable
    {
        public FreehandSliderToolboxGroup()
            : base()
        {
        }

        public readonly BindableInt DisplayTolerance = new BindableInt(90)
        {
            MinValue = 5,
            MaxValue = 100
        };

        public readonly BindableInt DisplayCornerThreshold = new BindableInt(40)
        {
            MinValue = 5,
            MaxValue = 100
        };

        public readonly BindableInt DisplayCircleThreshold = new BindableInt(30)
        {
            MinValue = 0,
            MaxValue = 100
        };

        private SidebarSlider<int, RoundedSliderBar<int>> toleranceSlider = null!;
        private SidebarSlider<int, RoundedSliderBar<int>> cornerThresholdSlider = null!;
        private SidebarSlider<int, RoundedSliderBar<int>> circleThresholdSlider = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    toleranceSlider = new SidebarSlider<int, RoundedSliderBar<int>>
                    {
                        Current = DisplayTolerance
                    },
                    cornerThresholdSlider = new SidebarSlider<int, RoundedSliderBar<int>>
                    {
                        Current = DisplayCornerThreshold
                    },
                    circleThresholdSlider = new SidebarSlider<int, RoundedSliderBar<int>>
                    {
                        Current = DisplayCircleThreshold
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            DisplayTolerance.BindValueChanged(tolerance =>
            {
                toleranceSlider.LabelText = $"Control Point Spacing: {tolerance.NewValue:N0}";
            }, true);

            DisplayCornerThreshold.BindValueChanged(threshold =>
            {
                cornerThresholdSlider.LabelText = $"Corner Threshold: {threshold.NewValue:N0}";
            }, true);

            DisplayCircleThreshold.BindValueChanged(threshold =>
            {
                circleThresholdSlider.LabelText = $"Perfect Curve Threshold: {threshold.NewValue:N0}";
            }, true);
        }
    }
}
