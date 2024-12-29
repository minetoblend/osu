// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class ControlPointSelectionManager : Component
    {
        private readonly List<ControlPoint> selectedControlPoints = new List<ControlPoint>();

        public IReadOnlyCollection<ControlPoint> Selection => selectedControlPoints;

        public event Action<ControlPointSelectionEvent>? SelectionChanged;

        public bool IsSelected(ControlPoint controlPoint) => selectedControlPoints.Contains(controlPoint);

        public void Select(ControlPoint controlPoint)
        {
            if (selectedControlPoints.Contains(controlPoint))
                return;

            selectedControlPoints.Add(controlPoint);

            SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, true));
        }

        public void Deselect(ControlPoint controlPoint)
        {
            if (selectedControlPoints.Remove(controlPoint))
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, true));
        }

        public void ToggleSelection(ControlPoint controlPoint)
        {
            if (IsSelected(controlPoint))
                Deselect(controlPoint);
            else
                Select(controlPoint);
        }

        public void SetSelection(IEnumerable<ControlPoint> controlPoints)
        {
            var toSelect = controlPoints.Except(selectedControlPoints);
            var toDeselect = selectedControlPoints.Except(controlPoints);

            foreach (var controlPoint in toDeselect)
                Deselect(controlPoint);

            foreach (var controlPoint in toSelect)
                Select(controlPoint);
        }

        public void Clear()
        {
            var controlPoints = selectedControlPoints.ToList();

            selectedControlPoints.Clear();

            foreach (var controlPoint in controlPoints)
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, false));
        }
    }

    public readonly record struct ControlPointSelectionEvent(ControlPoint ControlPoint, bool Selected);
}
