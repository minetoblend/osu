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
using osu.Game.Extensions;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class ControlPointSelectionManager : Component, IKeyBindingHandler<PlatformAction>
    {
        private readonly HashSet<ControlPoint> selection = new HashSet<ControlPoint>(ReferenceEqualityComparer.Instance);

        public IReadOnlyCollection<ControlPoint> Selection => selection;

        public bool AnySelected() => selection.Count > 0;

        public event Action<ControlPointSelectionEvent>? SelectionChanged;

        public bool IsSelected(ControlPoint controlPoint) => selection.Contains(controlPoint);

        public void Select(ControlPoint controlPoint)
        {
            if (selection.Add(controlPoint))
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, true));
        }

        public void Deselect(ControlPoint controlPoint)
        {
            if (selection.Remove(controlPoint))
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
            var toSelect = controlPoints.Except(selection).ToList();
            var toDeselect = selection.Except(controlPoints).ToList();

            selection.Clear();
            selection.AddRange(controlPoints);

            foreach (var controlPoint in toDeselect)
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, false));

            foreach (var controlPoint in toSelect)
                SelectionChanged?.Invoke(new ControlPointSelectionEvent(controlPoint, true));
        }

        public void SelectAll() => SetSelection(beatmap.ControlPointInfo.AllControlPoints);

        public void Clear()
        {
            foreach (var controlPoint in selection.ToList())
                Deselect(controlPoint);
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
