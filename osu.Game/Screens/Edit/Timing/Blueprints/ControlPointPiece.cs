// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
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

        protected override bool OnDragStart(DragStartEvent e)
        {
            changeHandler?.BeginChange();
            return true;
        }

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        protected override void OnDrag(DragEvent e)
        {
            var snapResult = snapProvider.FindSnappedPositionAndTime(e.ScreenSpaceMousePosition, EnableSnapping(e) ? SnapType.All : SnapType.None);

            if (snapResult.Time != null)
            {
                double time = snapResult.Time.Value;

                var snapTarget = GetContainingInputManager()?.HoveredDrawables.OfType<ControlPointPiece>().FirstOrDefault(it => it != this);

                if (snapTarget != null)
                    time = snapTarget.Blueprint.ControlPoint.Time;

                time = Math.Clamp(time, 0, editorClock.TrackLength);

                Blueprint.ControlPoint.Time = time;
            }
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            changeHandler?.EndChange();
        }
    }
}
