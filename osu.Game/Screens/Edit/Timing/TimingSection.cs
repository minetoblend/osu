// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Beatmaps.Timing;
using osu.Game.Graphics.UserInterfaceV2;

namespace osu.Game.Screens.Edit.Timing
{
    internal partial class TimingSection : Section<TimingControlPoint>
    {
        private LabelledTimeSignature timeSignature = null!;
        private LabelledSwitchButton omitBarLine = null!;
        private BPMTextBox bpmTextEntry = null!;

        private TimingPointSelectionFacade? selectionFacade;

        [BackgroundDependencyLoader]
        private void load()
        {
            Flow.AddRange(new Drawable[]
            {
                // new TapTimingControl(),
                bpmTextEntry = new BPMTextBox(),
                timeSignature = new LabelledTimeSignature
                {
                    Label = "Time Signature"
                },
                omitBarLine = new LabelledSwitchButton { Label = "Skip Bar Line" },
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bpmTextEntry.Current.BindValueChanged(_ => saveChanges());
            omitBarLine.Current.BindValueChanged(_ => saveChanges());
            timeSignature.Current.BindValueChanged(_ => saveChanges());

            void saveChanges()
            {
                if (!isRebinding) ChangeHandler?.SaveState();
            }
        }

        private bool isRebinding;

        protected override void OnControlPointsChanged(ValueChangedEvent<IReadOnlyList<TimingControlPoint>> points)
        {
            selectionFacade?.Expire();

            if (points.NewValue.Count > 0)
            {
                isRebinding = true;

                AddInternal(selectionFacade = new TimingPointSelectionFacade(points.NewValue));

                bpmTextEntry.Bindable = selectionFacade.BeatLengthBindable;

                timeSignature.Current = selectionFacade.TimeSignatureBindable;

                timeSignature.Indeterminate.UnbindBindings();
                timeSignature.Indeterminate.BindTo(selectionFacade.TimeSignatureBindable.Indeterminate);

                omitBarLine.Current = selectionFacade.OmitFirstBarLineBindable;

                omitBarLine.Indeterminate.UnbindBindings();
                omitBarLine.Indeterminate.BindTo(selectionFacade.OmitFirstBarLineBindable.Indeterminate);

                isRebinding = false;
            }
            else
            {
                selectionFacade = null;
            }
        }

        private partial class BPMTextBox : LabelledTextBox
        {
            private readonly BindableNumber<double> beatLengthBindable = new TimingControlPoint().BeatLengthBindable;

            private readonly BindableBool indeterminate = new BindableBool();

            public BPMTextBox()
            {
                Label = "BPM";
                SelectAllOnFocus = true;

                OnCommit += (_, isNew) =>
                {
                    if (!isNew) return;

                    try
                    {
                        if (double.TryParse(Current.Value, out double doubleVal) && doubleVal > 0)
                        {
                            beatLengthBindable.Value = beatLengthToBpm(doubleVal);
                        }
                    }
                    catch
                    {
                        // TriggerChange below will restore the previous text value on failure.
                    }

                    // This is run regardless of parsing success as the parsed number may not actually trigger a change
                    // due to bindable clamping. Even in such a case we want to update the textbox to a sane visual state.
                    beatLengthBindable.TriggerChange();
                };

                indeterminate.BindValueChanged(_ => updateDisplayValue());

                beatLengthBindable.BindValueChanged(_ => updateDisplayValue(), true);
            }

            private void updateDisplayValue()
            {
                Current.Value = indeterminate.Value ? "Mixed" : beatLengthToBpm(beatLengthBindable.Value).ToString("N2");
            }

            public Bindable<double> Bindable
            {
                get => beatLengthBindable;
                set
                {
                    // incoming will be beat length, not bpm
                    beatLengthBindable.UnbindBindings();
                    indeterminate.UnbindBindings();

                    beatLengthBindable.BindTo(value);

                    if (value is SelectionBindable<double> facadeProperty)
                        indeterminate.BindTo(facadeProperty.Indeterminate);
                }
            }
        }

        private static double beatLengthToBpm(double beatLength) => 60000 / beatLength;
    }

    public partial class TimingPointSelectionFacade : ControlPointSelectionFacade<TimingControlPoint>
    {
        public TimingPointSelectionFacade(IReadOnlyList<TimingControlPoint> controlPoints)
            : base(controlPoints)
        {
            BeatLengthBindable = CreateProperty(static t => t.BeatLengthBindable);
            TimeSignatureBindable = CreateProperty(static t => t.TimeSignatureBindable, TimeSignature.SimpleQuadruple);
            OmitFirstBarLineBindable = CreateProperty(static t => t.OmitFirstBarLineBindable);
        }

        public readonly SelectionBindable<double> BeatLengthBindable;

        public readonly SelectionBindable<TimeSignature> TimeSignatureBindable;

        public readonly SelectionBindable<bool> OmitFirstBarLineBindable;
    }
}
