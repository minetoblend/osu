// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using Vector2 = osuTK.Vector2;

namespace osu.Game.Rulesets.Edit.UI
{
    public partial class SidebarSlider<T, TSlider> : CompositeDrawable, IHasCurrentValue<T>
        where T : struct, INumber<T>, IMinMaxValue<T>
        where TSlider : RoundedSliderBar<T>, new()
    {
        private readonly OsuSpriteText label;
        private readonly TSlider slider;

        private LocalisableString labelText;

        public LocalisableString LabelText
        {
            get => labelText;
            set
            {
                if (value == labelText)
                    return;

                labelText = value;

                label.Text = value;
            }
        }

        public Bindable<T> Current
        {
            get => slider.Current;
            set => slider.Current = value;
        }

        /// <summary>
        /// A custom step value for each key press which actuates a change on this control.
        /// </summary>
        public float KeyboardStep
        {
            get => slider.KeyboardStep;
            set => slider.KeyboardStep = value;
        }

        public override bool HandlePositionalInput => true;

        public SidebarSlider()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(0f, 10f),
                Children = new Drawable[]
                {
                    label = new OsuSpriteText(),
                    slider = new TSlider
                    {
                        RelativeSizeAxes = Axes.X,
                    },
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindDisabledChanged(disabled =>
            {
                slider.Alpha = disabled ? 0.3f : 1;
            });

            SchedulerAfterChildren.Add(() => slider.FinishTransforms(true));
        }
    }

    public partial class SidebarSlider<T> : SidebarSlider<T, RoundedSliderBar<T>>
        where T : struct, INumber<T>, IMinMaxValue<T>
    {
    }
}
