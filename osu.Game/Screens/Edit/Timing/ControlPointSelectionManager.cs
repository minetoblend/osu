// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class ControlPointSelectionManager : Component, IKeyBindingHandler<PlatformAction>
    {
        private readonly HashSet<ControlPoint> selectedControlPoints = new HashSet<ControlPoint>(ReferenceEqualityComparer.Instance);

        public IReadOnlyCollection<ControlPoint> Selection => selectedControlPoints;

        public event Action<ControlPointSelectionEvent>? SelectionChanged;

        public bool IsSelected(ControlPoint controlPoint) => selectedControlPoints.Contains(controlPoint);

        public void Select(ControlPoint controlPoint)
        {
            if (selectedControlPoints.Add(controlPoint))
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, true));
        }

        public void Deselect(ControlPoint controlPoint)
        {
            if (selectedControlPoints.Remove(controlPoint))
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, false));
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
            var toSelect = controlPoints.Except(selectedControlPoints).ToList();
            var toDeselect = selectedControlPoints.Except(controlPoints).ToList();

            foreach (var controlPoint in toDeselect)
                Deselect(controlPoint);

            foreach (var controlPoint in toSelect)
                Select(controlPoint);
        }

        public void SelectAll() => SetSelection(beatmap.ControlPointInfo.AllControlPoints);

        public void Clear()
        {
            var controlPoints = selectedControlPoints.ToList();

            selectedControlPoints.Clear();

            foreach (var controlPoint in controlPoints)
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, false));
        }

        public void SelectFromMouseDown(ControlPoint controlPoint, UIEvent evt)
        {
            if (evt.ControlPressed)
                ToggleSelection(controlPoint);
            else if (!IsSelected(controlPoint))
                SetSelection(new[] { controlPoint });
        }

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmap.ControlPointInfo.ControlPointRemoved += Deselect;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            beatmap.ControlPointInfo.ControlPointRemoved -= Deselect;
        }

        public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
        {
            if (e.Action == PlatformAction.SelectAll)
            {
                SelectAll();
                return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
        {
        }
    }

    public readonly record struct ControlPointSelectionEvent(ControlPoint ControlPoint, bool Selected);
}
