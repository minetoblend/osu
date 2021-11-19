// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Osu.Objects
{
    public class HitObjectWithPath : OsuHitObject, IHasPath
    {
        public virtual double EndTime => StartTime + Path.Distance / Velocity;

        [JsonIgnore]
        public double Duration
        {
            get => EndTime - StartTime;
            set => throw new System.NotSupportedException($"Adjust Stream path instead via instead"); // can be implemented if/when needed.
        }

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
                }
            }
        }

        public double Distance => Path.Distance;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);

            double scoringDistance = BASE_SCORING_DISTANCE * difficulty.SliderMultiplier * DifficultyControlPoint.SliderVelocity;

            Velocity = scoringDistance / timingPoint.BeatLength;
        }

        /// <summary>
        /// Velocity of this <see cref="Slider"/>.
        /// </summary>
        public double Velocity { get; protected set; }
    }
}
