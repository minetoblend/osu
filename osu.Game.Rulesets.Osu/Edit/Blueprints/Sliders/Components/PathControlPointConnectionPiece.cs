// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders.Components
{
    /// <summary>
    /// A visualisation of the line between two <see cref="PathControlPointPiece"/>s.
    /// </summary>
    public class PathControlPointConnectionPiece : CompositeDrawable
    {
        public readonly PathControlPoint ControlPoint;

        private readonly Path path;
        private readonly HitObjectWithPath hitObject;
        public int ControlPointIndex { get; set; }

        private IBindable<Vector2> sliderPosition;
        private IBindable<int> pathVersion;

        public PathControlPointConnectionPiece(HitObjectWithPath hitObject, int controlPointIndex)
        {
            this.hitObject = hitObject;
            ControlPointIndex = controlPointIndex;

            Origin = Anchor.Centre;
            AutoSizeAxes = Axes.Both;

            ControlPoint = hitObject.Path.ControlPoints[controlPointIndex];

            InternalChild = path = new SmoothPath
            {
                Anchor = Anchor.Centre,
                PathRadius = 1
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            sliderPosition = hitObject.PositionBindable.GetBoundCopy();
            sliderPosition.BindValueChanged(_ => updateConnectingPath());

            pathVersion = hitObject.Path.Version.GetBoundCopy();
            pathVersion.BindValueChanged(_ => updateConnectingPath());

            updateConnectingPath();
        }

        /// <summary>
        /// Updates the path connecting this control point to the next one.
        /// </summary>
        private void updateConnectingPath()
        {
            Position = hitObject.StackedPosition + ControlPoint.Position;

            path.ClearVertices();

            int nextIndex = ControlPointIndex + 1;
            if (nextIndex == 0 || nextIndex >= hitObject.Path.ControlPoints.Count)
                return;

            path.AddVertex(Vector2.Zero);
            path.AddVertex(hitObject.Path.ControlPoints[nextIndex].Position - ControlPoint.Position);

            path.OriginPosition = path.PositionInBoundingBox(Vector2.Zero);
        }
    }
}
