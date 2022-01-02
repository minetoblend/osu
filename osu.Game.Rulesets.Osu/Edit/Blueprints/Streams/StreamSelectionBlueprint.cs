// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles.Components;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders.Components;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;
using osu.Game.Screens.Edit;
using osu.Game.Screens.Edit.Compose;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Streams
{
    public class StreamSelectionBlueprint : OsuSelectionBlueprint<HitCircleStream>
    {
        protected new DrawableHitCircleStream DrawableObject => (DrawableHitCircleStream)base.DrawableObject;

        [CanBeNull]
        protected PathControlPointVisualiser ControlPointVisualiser { get; private set; }

        [Resolved(CanBeNull = true)]
        private HitObjectComposer composer { get; set; }

        [Resolved(CanBeNull = true)]
        private IPlacementHandler placementHandler { get; set; }

        [Resolved(CanBeNull = true)]
        private EditorBeatmap editorBeatmap { get; set; }

        [Resolved(CanBeNull = true)]
        private IEditorChangeHandler changeHandler { get; set; }

        [Resolved(CanBeNull = true)]
        private BindableBeatDivisor beatDivisor { get; set; }

        public override Quad SelectionQuad
        {
            get
            {
                if (!circleContainer.Any())
                    return new Quad();

                Vector2 minPosition = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 maxPosition = new Vector2(float.MinValue, float.MinValue);

                foreach (var c in circleContainer)
                {
                    minPosition = Vector2.ComponentMin(minPosition, c.ScreenSpaceDrawQuad.TopLeft);
                    maxPosition = Vector2.ComponentMax(maxPosition, c.ScreenSpaceDrawQuad.BottomRight);
                }

                Vector2 size = maxPosition - minPosition;

                return new Quad(minPosition.X, minPosition.Y, size.X, size.Y);
            }
        }

        private readonly BindableList<PathControlPoint> controlPoints = new BindableList<PathControlPoint>();
        private readonly IBindable<int> pathVersion = new Bindable<int>();

        private Container<HitCirclePiece> circleContainer;

        public StreamSelectionBlueprint(HitCircleStream stream)
            : base(stream)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                circleContainer = new Container<HitCirclePiece> { RelativeSizeAxes = Axes.Both },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            controlPoints.BindTo(HitObject.Path.ControlPoints);

            pathVersion.BindTo(HitObject.Path.Version);
            pathVersion.BindValueChanged(_ => updatePath());
        }

        private void updatePath()
        {
            HitObject.Path.ExpectedDistance.Value = composer?.GetSnappedDistanceFromDistance(HitObject, (float)HitObject.Path.CalculatedDistance) ?? (float)HitObject.Path.CalculatedDistance;
            editorBeatmap?.Update(HitObject);
            circleContainer.Clear();

            foreach (var circle in HitObject.NestedHitObjects.OfType<HitCircle>())
            {
                var piece = new HitCirclePiece();
                circleContainer.Add(piece);
                piece.UpdateFrom(circle);
            }
        }

        public override bool HandleQuickDeletion()
        {
            var hoveredControlPoint = ControlPointVisualiser?.Pieces.FirstOrDefault(p => p.IsHovered);

            if (hoveredControlPoint == null)
                return false;

            hoveredControlPoint.IsSelected.Value = true;
            ControlPointVisualiser.DeleteSelected();
            return true;
        }

        protected override void Update()
        {
            base.Update();

            if (IsSelected)
                updatePath();
        }

        protected override void OnSelected()
        {
            AddInternal(ControlPointVisualiser = new PathControlPointVisualiser(HitObject, true)
            {
                RemoveControlPointsRequested = removeControlPoints
            });

            base.OnSelected();
        }

        protected override void OnDeselected()
        {
            base.OnDeselected();

            // throw away frame buffers on deselection.
            ControlPointVisualiser?.Expire();
            ControlPointVisualiser = null;

            circleContainer.Clear();
        }

        private Vector2 rightClickPosition;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            switch (e.Button)
            {
                case MouseButton.Right:
                    rightClickPosition = e.MouseDownPosition;
                    return false; // Allow right click to be handled by context menu

                case MouseButton.Left:
                    if (e.ControlPressed && IsSelected)
                    {
                        changeHandler?.BeginChange();
                        placementControlPoint = addControlPoint(e.MousePosition);
                        ControlPointVisualiser?.SetSelectionTo(placementControlPoint);
                        return true; // Stop input from being handled and modifying the selection
                    }

                    break;
            }

            return false;
        }

        [CanBeNull]
        private PathControlPoint placementControlPoint;

        protected override bool OnDragStart(DragStartEvent e) => placementControlPoint != null;

        protected override void OnDrag(DragEvent e)
        {
            if (placementControlPoint != null)
                placementControlPoint.Position = e.MousePosition - HitObject.Position;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (placementControlPoint != null)
            {
                placementControlPoint = null;
                changeHandler?.EndChange();
            }
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (!IsSelected)
                return false;

            return false;
        }

        private PathControlPoint addControlPoint(Vector2 position)
        {
            position -= HitObject.Position;

            int insertionIndex = 0;
            float minDistance = float.MaxValue;

            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                float dist = new Line(controlPoints[i].Position, controlPoints[i + 1].Position).DistanceToPoint(position);

                if (dist < minDistance)
                {
                    insertionIndex = i + 1;
                    minDistance = dist;
                }
            }

            var pathControlPoint = new PathControlPoint { Position = position };

            // Move the control points from the insertion index onwards to make room for the insertion
            controlPoints.Insert(insertionIndex, pathControlPoint);

            return pathControlPoint;
        }

        private void removeControlPoints(List<PathControlPoint> toRemove)
        {
            // Ensure that there are any points to be deleted
            if (toRemove.Count == 0)
                return;

            foreach (var c in toRemove)
            {
                // The first control point in the slider must have a type, so take it from the previous "first" one
                // Todo: Should be handled within SliderPath itself
                if (c == controlPoints[0] && controlPoints.Count > 1 && controlPoints[1].Type == null)
                    controlPoints[1].Type = controlPoints[0].Type;

                controlPoints.Remove(c);
            }

            // If there are 0 or 1 remaining control points, the slider is in a degenerate (single point) form and should be deleted
            if (controlPoints.Count <= 1 || !HitObject.Path.HasValidLength)
            {
                placementHandler?.Delete(HitObject);
                return;
            }

            // The path will have a non-zero offset if the head is removed, but sliders don't support this behaviour since the head is positioned at the slider's position
            // So the slider needs to be offset by this amount instead, and all control points offset backwards such that the path is re-positioned at (0, 0)
            Vector2 first = controlPoints[0].Position;
            foreach (var c in controlPoints)
                c.Position -= first;
            HitObject.Position += first;
        }

        public override MenuItem[] ContextMenuItems => new MenuItem[]
        {
            new OsuMenuItem("Add control point", MenuItemType.Standard, () => addControlPoint(rightClickPosition)),
            new OsuMenuItem("Update beatsnap divisor", MenuItemType.Standard, updateSnapDivisor)
        };

        private void updateSnapDivisor()
        {
            HitObject.BeatDivisor = editorBeatmap.BeatDivisor;
            editorBeatmap.Update(HitObject);
        }

        // Always refer to the drawable object's slider body so subsequent movement deltas are calculated with updated positions.
        public override Vector2 ScreenSpaceSelectionPoint => DrawableObject.ToScreenSpace(DrawableObject.HitObject.Position);

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) =>
            circleContainer.Any(circle => circle.ReceivePositionalInputAt(screenSpacePos)) || ControlPointVisualiser?.Pieces.Any(p => p.ReceivePositionalInputAt(screenSpacePos)) == true;
    }
}
