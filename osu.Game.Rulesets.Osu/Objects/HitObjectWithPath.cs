// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using Newtonsoft.Json;
using osu.Framework.Caching;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Osu.Objects
{
    public abstract class HitObjectWithPath : OsuHitObject, IHasPath
    {
        public virtual double EndTime => StartTime + Path.Distance / Velocity;

        private readonly Cached<Vector2> endPositionCache = new Cached<Vector2>();

        public Vector2 StackedPositionAt(double t) => StackedPosition + this.CurvePositionAt(t);

        public override Vector2 EndPosition => endPositionCache.IsValid ? endPositionCache.Value : endPositionCache.Value = Position + this.CurvePositionAt(1);

        private readonly SliderPath path = new SliderPath();

        public SliderPath Path
        {
            get => path;
            set
            {
                path.ControlPoints.Clear();
                path.ExpectedDistance.Value = null;

                if (value != null)
                {
                    path.ControlPoints.AddRange(value.ControlPoints.Select(c => new PathControlPoint(c.Position, c.Type)));
                    path.ExpectedDistance.Value = value.ExpectedDistance.Value;
                }
            }
        }

        public double Distance => Path.Distance;

        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                updateNestedPositions();
            }
        }

        [JsonIgnore]
        public virtual double Duration
        {
            get => EndTime - StartTime;
            set => throw new System.NotSupportedException();
        }

        protected virtual void updateNestedPositions()
        {
            endPositionCache.Invalidate();
        }

        /// <summary>
        /// Velocity of this <see cref="Slider"/>.
        /// </summary>
        public double Velocity { get; protected set; }
    }
}
