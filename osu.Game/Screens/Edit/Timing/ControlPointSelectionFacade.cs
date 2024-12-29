// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Threading;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class ControlPointSelectionFacade<T> : Component
        where T : ControlPoint
    {
        public ControlPointSelectionFacade(IReadOnlyCollection<T> controlPoints)
        {
            ControlPoints = controlPoints;

            Time = CreateProperty(static cp => cp.TimeBindable);
        }

        public new readonly SelectionBindable<double> Time;

        protected readonly IReadOnlyCollection<T> ControlPoints;

        protected SelectionBindable<TValue> CreateProperty<TValue>(Func<T, Bindable<TValue>> getBindable, TValue defaultValue = default)
            where TValue : IEquatable<TValue> => new SelectionBindable<TValue>(Scheduler, ControlPoints.Select(getBindable).ToList(), defaultValue);
    }

    public class SelectionBindable<T> : Bindable<T>
        where T : IEquatable<T>
    {
        public SelectionBindable(Scheduler scheduler, IReadOnlyList<Bindable<T>> bindables, T defaultValue = default)
            : base(defaultValue)
        {
            this.scheduler = scheduler;
            this.bindables = bindables;

            bindEvents();
        }

        private readonly Scheduler scheduler;

        private readonly IReadOnlyList<Bindable<T>> bindables;

        public Bindable<bool> Indeterminate = new BindableBool();

        private void bindEvents()
        {
            foreach (var bindable in bindables)
                bindable.ValueChanged += childValueChanged;

            computeValue();

            ValueChanged += evt =>
            {
                if (isUpdatingValue)
                    return;

                foreach (var bindable in bindables)
                    bindable.Value = Value;
            };
        }

        private void childValueChanged(ValueChangedEvent<T> _) => scheduler.AddOnce(computeValue);

        private void computeValue()
        {
            if (bindables.Count == 0)
            {
                setValue(Default, true);
                return;
            }

            var value = bindables.First().Value;

            for (int i = 1; i < bindables.Count; i++)
            {
                if (!bindables[i].Value.Equals(value))
                {
                    setValue(Default, true);
                    return;
                }
            }

            setValue(value, false);
        }

        private bool isUpdatingValue = false;

        private void setValue(T value, bool indeterminate)
        {
            isUpdatingValue = true;
            Indeterminate.Value = indeterminate;
            Value = value;
            isUpdatingValue = false;
        }

        public override void UnbindEvents()
        {
            base.UnbindEvents();

            foreach (var bindable in bindables)
                bindable.ValueChanged -= childValueChanged;
        }
    }
}
