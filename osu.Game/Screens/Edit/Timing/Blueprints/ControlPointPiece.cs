// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Edit;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public abstract partial class ControlPointPiece : CompositeDrawable
    {
        protected abstract Drawable CreateContent();

        protected ControlPointPiece(ControlPointBlueprint blueprint)
        {
            Blueprint = blueprint;

            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;

            AddInternal(CreateContent());
        }

        protected readonly ControlPointBlueprint Blueprint;

        public ControlPoint ControlPoint => Blueprint.ControlPoint;

        internal virtual void SelectionChanged(bool selected)
        {
        }

        protected override bool OnHover(HoverEvent e) => true;

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        [Resolved]
        private IPositionSnapProvider snapProvider { get; set; } = null!;

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        protected virtual bool EnableSnapping(UIEvent e) => !e.ShiftPressed;

        protected override bool OnClick(ClickEvent e)
        {
            attemptSelectionFromMouse(e);
            return true;
        }

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Resolved]
        private ControlPointSelectionManager? selectionManager { get; set; }

        private void attemptSelectionFromMouse(MouseEvent e)
        {
            if (selectionManager == null)
                return;

            if (e.ControlPressed)
                selectionManager.ToggleSelection(ControlPoint);
            else if (!Blueprint.Selected.Value)
                selectionManager.SetSelection(new[] { ControlPoint });
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            attemptSelectionFromMouse(e);

            changeHandler?.BeginChange();
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            var snapResult = snapProvider.FindSnappedPositionAndTime(e.ScreenSpaceMousePosition, EnableSnapping(e) ? SnapType.All : SnapType.None);

            if (snapResult.Time != null)
            {
                double time = snapResult.Time.Value;

                var snapTarget = GetContainingInputManager()?.HoveredDrawables.OfType<ControlPointPiece>().FirstOrDefault(it => it != this);

                if (snapTarget != null)
                    time = snapTarget.ControlPoint.Time;

                time = Math.Clamp(time, 0, editorClock.TrackLength);

                // TODO: update entire selection instead of just current point
                ControlPoint.Time = time;
            }
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            changeHandler?.EndChange();

            updateControlPointGroup();
        }

        private void updateControlPointGroup()
        {
            // unfortunately control point groups are a thing that exists, so we need to make sure detach the control point from its group
            // and readd it to a new group after we're done changing the control point's time

            var controlPoint = ControlPoint;

            if (controlPoint.Group != null && Precision.AlmostEquals(ControlPoint.Time, controlPoint.Group.Time))
                return;

            bool wasSelected = Blueprint.Selected.Value;

            var group = controlPoint.Group;

            if (group != null)
            {
                group.Remove(controlPoint);

                if (group.ControlPoints.Count == 0)
                    beatmap.ControlPointInfo.RemoveGroup(group);
            }

            beatmap.ControlPointInfo.GroupAt(controlPoint.Time, true).Add(controlPoint);

            // since removing a control point from the group will automatically deselect it, we need to re-add it to the selection
            if (wasSelected)
                selectionManager?.Select(controlPoint);
        }
    }
}
