// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Performance;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public class ControlPointLifetimeEntry : LifetimeEntry
    {
        public event Action? Invalidated;
        public readonly ControlPoint Start;

        public ControlPointLifetimeEntry(ControlPoint start)
        {
            Start = start;
            LifetimeStart = Start.Time;

            bindEvents();
        }

        private ControlPoint? end;

        public ControlPoint? End
        {
            get => end;
            set
            {
                UnbindEvents();

                end = value;

                bindEvents();

                refreshLifetimes();
            }
        }

        private bool wasBound;

        private void bindEvents()
        {
            UnbindEvents();

            Start.Changed += onChanged;

            if (End != null)
                End.Changed += onChanged;

            wasBound = true;
        }

        public void UnbindEvents()
        {
            if (!wasBound)
                return;

            Start.Changed -= onChanged;

            if (End != null)
                End.Changed -= onChanged;

            wasBound = false;
        }

        private void onChanged(ControlPoint controlPoint) => refreshLifetimes();

        private void refreshLifetimes()
        {
            LifetimeStart = Start.Time;
            LifetimeEnd = End?.Time ?? double.MaxValue;

            Invalidated?.Invoke();
        }
    }
}
