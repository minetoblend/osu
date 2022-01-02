// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading;
using Newtonsoft.Json;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Osu.Objects
{
    public class HitCircleStream : HitObjectWithPath
    {
        /// <summary>
        /// Positional distance that results in a duration of one second, before any speed adjustments.
        /// </summary>
        private const float base_scoring_distance = 100;

        public override Judgement CreateJudgement() => new IgnoreJudgement();

        [JsonIgnore]
        public double TickDistance { get; private set; }

        public int BeatDivisor = 4;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);

            double scoringDistance = base_scoring_distance * difficulty.SliderMultiplier * DifficultyControlPoint.SliderVelocity;

            Velocity = scoringDistance / timingPoint.BeatLength;
            TickDistance = scoringDistance / difficulty.SliderTickRate;
        }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            foreach (var e in SliderEventGenerator.Generate(StartTime, Duration, Velocity, TickDistance / BeatDivisor, Path.Distance, 1, LegacyLastTickOffset, cancellationToken))
            {
                switch (e.Type)
                {
                    case SliderEventType.Tick:
                    case SliderEventType.Head:
                    case SliderEventType.Tail:
                        AddNested(new HitCircle()
                        {
                            Samples = Samples,
                            StartTime = e.Time,
                            Position = Position + Path.PositionAt(e.PathProgress),
                        });
                        break;
                }
            }
        }

        public double? LegacyLastTickOffset { get; set; }
    }
}
