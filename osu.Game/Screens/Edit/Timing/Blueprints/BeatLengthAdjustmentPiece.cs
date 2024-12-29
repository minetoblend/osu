// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit.Compose.Components.Timeline;
using osuTK.Input;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class BeatLengthAdjustmentPiece : CompositeDrawable, IHasTooltip
    {
        [Resolved]
        private Timeline timeline { get; set; } = null!;

        [Resolved]
        private IEditorChangeHandler? changeHandler { get; set; }

        private readonly TimingPointBlueprint blueprint;

        public virtual LocalisableString TooltipText { get; private set; }

        public BeatLengthAdjustmentPiece(TimingPointBlueprint blueprint)
        {
            this.blueprint = blueprint;
            AlwaysPresent = true;
        }

        private Drawable dragHandle = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            Width = 10;
            Origin = Anchor.TopCentre;

            AddInternal(dragHandle = new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Width = 2,
                Alpha = 0,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.25f
                }
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateHandleVisibility();

            return false;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            updateHandleVisibility();
        }

        private bool controlPressed;

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Key == Key.ControlLeft)
            {
                controlPressed = true;
                updateHandleVisibility();
            }

            return false;
        }

        protected override void OnKeyUp(KeyUpEvent e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.ControlLeft)
            {
                controlPressed = false;
                updateHandleVisibility();
            }
        }

        private void updateHandleVisibility()
        {
            if (!IsHovered || !controlPressed || Precision.AlmostEquals(timeAtMousePosition, blueprint.ControlPoint.Time))
                dragHandle.Hide();
            else
                dragHandle.Show();
        }

        private InputManager inputManager = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            inputManager = GetContainingInputManager()!;
        }

        private double timeAtMousePosition;

        protected override void Update()
        {
            base.Update();

            var snapResult = timeline.FindSnappedPositionAndTime(inputManager.CurrentState.Mouse.Position);

            if (snapResult.Time != null && snapResult.Time != timeAtMousePosition)
            {
                timeAtMousePosition = Math.Clamp(snapResult.Time.Value, blueprint.StartTimeBindable.Value, blueprint.EndTimeBindable.Value);
                updateHandleVisibility();
            }

            X = timeline.PositionAtTime(timeAtMousePosition - blueprint.ControlPoint.Time);
        }

        private double initialBeatLength;
        private double dragOffset;

        protected override bool OnDragStart(DragStartEvent e)
        {
            if (!e.ControlPressed || Precision.AlmostEquals(timeAtMousePosition, blueprint.ControlPoint.Time))
                return false;

            initialBeatLength = blueprint.ControlPoint.BeatLength;
            dragOffset = timeAtMousePosition - blueprint.ControlPoint.Time;

            changeHandler?.BeginChange();

            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            base.OnDrag(e);

            var snapResult = timeline.FindSnappedPositionAndTime(e.ScreenSpaceMousePosition, SnapType.None);

            double time = snapResult.Time ?? timeAtMousePosition;

            var snapTarget = inputManager.HoveredDrawables.OfType<ControlPointPiece>().FirstOrDefault();
            if (snapTarget != null)
                time = snapTarget.ControlPoint.Time;

            double ratio = (time - blueprint.ControlPoint.Time) / dragOffset;

            blueprint.ControlPoint.BeatLength = Math.Clamp(initialBeatLength * ratio, 100, 10000);
            timeline.InvalidateTicks();

            TooltipText = $"{blueprint.ControlPoint.BPM:N2} bpm";
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            changeHandler?.EndChange();

            TooltipText = string.Empty;
        }
    }
}
