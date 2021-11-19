// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders.Components;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Streams
{
    public class StreamPlacementBlueprint : PlacementBlueprint
    {
        public new Stream HitObject => (Stream)base.HitObject;

        private PathControlPointVisualiser controlPointVisualiser;

        private InputManager inputManager;

        private StreamPlacementState state;
        private PathControlPoint segmentStart;
        private PathControlPoint cursor;
        private int currentSegmentLength;

        [Resolved(CanBeNull = true)]
        private HitObjectComposer composer { get; set; }

        public StreamPlacementBlueprint()
            : base(new Stream())
        {
            RelativeSizeAxes = Axes.Both;

            HitObject.Path.ControlPoints.Add(segmentStart = new PathControlPoint(Vector2.Zero, PathType.Linear));
            currentSegmentLength = 1;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChildren = new Drawable[]
            {
                controlPointVisualiser = new PathControlPointVisualiser(HitObject, false)
            };

            setState(StreamPlacementState.Initial);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left)
                return base.OnMouseDown(e);

            switch (state)
            {
                case StreamPlacementState.Initial:
                    beginCurve();
                    break;

                case StreamPlacementState.Body:
                    if (canPlaceNewControlPoint(out var lastPoint))
                    {
                        // Place a new point by detatching the current cursor.
                        updateCursor();
                        cursor = null;
                    }
                    else
                    {
                        // Transform the last point into a new segment.
                        Debug.Assert(lastPoint != null);

                        segmentStart = lastPoint;
                        segmentStart.Type = PathType.Linear;

                        currentSegmentLength = 1;
                    }

                    break;
            }

            return false;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (state == StreamPlacementState.Body && e.Button == MouseButton.Right)
                endCurve();
            base.OnMouseUp(e);
        }

        private void beginCurve()
        {
            BeginPlacement(commitStart: true);
            setState(StreamPlacementState.Body);
        }

        private void endCurve()
        {
            updateStream();
            EndPlacement(HitObject.Path.HasValidLength);
        }

        protected override void Update()
        {
            base.Update();
            updateStream();

            updatePathType();
        }

        private void updatePathType()
        {
            switch (currentSegmentLength)
            {
                case 1:
                case 2:
                    segmentStart.Type = PathType.Linear;
                    break;

                case 3:
                    segmentStart.Type = PathType.PerfectCurve;
                    break;

                default:
                    segmentStart.Type = PathType.Bezier;
                    break;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            inputManager = GetContainingInputManager();
        }

        public override void UpdateTimeAndPosition(SnapResult result)
        {
            base.UpdateTimeAndPosition(result);

            switch (state)
            {
                case StreamPlacementState.Initial:
                    BeginPlacement();
                    HitObject.Position = ToLocalSpace(result.ScreenSpacePosition);
                    break;

                case StreamPlacementState.Body:
                    updateCursor();
                    break;
            }
        }

        private void updateCursor()
        {
            if (canPlaceNewControlPoint(out _))
            {
                // The cursor does not overlap a previous control point, so it can be added if not already existing.
                if (cursor == null)
                {
                    HitObject.Path.ControlPoints.Add(cursor = new PathControlPoint { Position = Vector2.Zero });

                    // The path type should be adjusted in the progression of updatePathType() (Linear -> PC -> Bezier).
                    currentSegmentLength++;
                    updatePathType();
                }

                // Update the cursor position.
                cursor.Position = ToLocalSpace(inputManager.CurrentState.Mouse.Position) - HitObject.Position;
            }
            else if (cursor != null)
            {
                // The cursor overlaps a previous control point, so it's removed.
                HitObject.Path.ControlPoints.Remove(cursor);
                cursor = null;

                // The path type should be adjusted in the reverse progression of updatePathType() (Bezier -> PC -> Linear).
                currentSegmentLength--;
                updatePathType();
            }
        }

        private bool canPlaceNewControlPoint([CanBeNull] out PathControlPoint lastPoint)
        {
            // We cannot rely on the ordering of drawable pieces, so find the respective drawable piece by searching for the last non-cursor control point.
            var last = HitObject.Path.ControlPoints.LastOrDefault(p => p != cursor);
            var lastPiece = controlPointVisualiser.Pieces.Single(p => p.ControlPoint == last);

            lastPoint = last;
            return lastPiece.IsHovered != true;
        }

        private void updateStream()
        {
            HitObject.Path.ExpectedDistance.Value = composer?.GetSnappedDistanceFromDistance(HitObject, (float)HitObject.Path.CalculatedDistance) ?? (float)HitObject.Path.CalculatedDistance;
            HitObject.DurationPerCircle = composer.GetBeatSnapDistanceAt(HitObject);
            HitObject.UpdateNestedHitObjects();
        }

        private void setState(StreamPlacementState newState)
        {
            state = newState;
        }

        private enum StreamPlacementState
        {
            Initial,
            Body,
        }
    }
}
