// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Screens.Edit.Timing
{
    internal partial class EffectSection : Section<EffectControlPoint>
    {
        private LabelledSwitchButton kiai = null!;

        private IndeterminateSliderWithTextBoxInput<double> scrollSpeedSlider = null!;

        private Bindable<double> indeterminateScrollSpeed = new EffectControlPoint().ScrollSpeedBindable;

        [BackgroundDependencyLoader]
        private void load()
        {
            Flow.AddRange(new Drawable[]
            {
                kiai = new LabelledSwitchButton { Label = "Kiai Time" },
                scrollSpeedSlider = new IndeterminateSliderWithTextBoxInput<double>("Scroll Speed", indeterminateScrollSpeed)
                {
                    KeyboardStep = 0.1f
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            kiai.Current.BindValueChanged(_ => saveChanges());
            scrollSpeedSlider.Current.BindValueChanged(_ => saveChanges());

            if (!Beatmap.BeatmapInfo.Ruleset.CreateInstance().EditorShowScrollSpeed)
                scrollSpeedSlider.Hide();

            void saveChanges()
            {
                if (!isRebinding) ChangeHandler?.SaveState();
            }
        }

        private bool isRebinding;

        private EffectPointSelectionFacade? selectionFacade;

        protected override void OnControlPointsChanged(ValueChangedEvent<IReadOnlyList<EffectControlPoint>> points)
        {
            scrollSpeedSlider.Current.ValueChanged -= updateControlPointFromSlider;

            selectionFacade?.Expire();

            if (points.NewValue.Count > 0)
            {
                isRebinding = true;

                AddInternal(selectionFacade = new EffectPointSelectionFacade(points.NewValue));

                kiai.Current = selectionFacade.KiaiModeBindable;
                kiai.Indeterminate.UnbindBindings();
                kiai.Indeterminate.BindTo(selectionFacade.KiaiModeBindable.Indeterminate);

                var scrollSpeedBindable = new BindableDouble
                {
                    MinValue = 0.01,
                    MaxValue = 10,
                    Precision = 0.01,
                    Value = selectionFacade.ScrollSpeedBindable.Value
                };

                scrollSpeedBindable.BindTo(selectionFacade.ScrollSpeedBindable);

                selectionFacade.ScrollSpeedBindable.BindValueChanged(e => scrollSpeedSlider.Current.Value = e.NewValue, true);

                scrollSpeedSlider.Current.ValueChanged += updateControlPointFromSlider;
                // at this point in time the above is enough to keep the slider control in sync with reality,
                // since undo/redo causes `OnControlPointChanged()` to fire.
                // whenever that stops being the case, or there is a possibility that the scroll speed could be changed
                // by something else other than this control, this code should probably be revisited to have a binding in the other direction, too.

                isRebinding = false;
            }
            else
            {
                selectionFacade = null;
            }
        }

        private void updateControlPointFromSlider(ValueChangedEvent<double?> scrollSpeed)
        {
            if (scrollSpeed.NewValue == null || selectionFacade == null || isRebinding)
                return;

            selectionFacade.ScrollSpeedBindable.Value = scrollSpeed.NewValue.Value;
        }

        private partial class EffectPointSelectionFacade : ControlPointSelectionFacade<EffectControlPoint>
        {
            public EffectPointSelectionFacade(IReadOnlyCollection<EffectControlPoint> controlPoints)
                : base(controlPoints)
            {
                KiaiModeBindable = CreateProperty(static cp => cp.KiaiModeBindable);
                ScrollSpeedBindable = CreateProperty(static cp => cp.ScrollSpeedBindable);
            }

            public readonly SelectionBindable<bool> KiaiModeBindable;

            public readonly SelectionBindable<double> ScrollSpeedBindable;
        }
    }
}
