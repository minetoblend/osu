// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Rulesets.Osu.Objects
{
    public class Stream : HitObjectWithPath
    {
        public float DurationPerCircle;

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimingControlPoint timingPoint = controlPointInfo.TimingPointAt(StartTime);

            double scoringDistance = BASE_SCORING_DISTANCE * difficulty.SliderMultiplier * DifficultyControlPoint.SliderVelocity;

            Velocity = scoringDistance / timingPoint.BeatLength;

            DurationPerCircle = (float)(timingPoint.BeatLength * 0.25);
        }

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            UpdateNestedHitObjects();
        }

        public void UpdateNestedHitObjects()
        {
            int numHitObjects = (int)(Duration / DurationPerCircle + 0.5);

            if (NestedHitObjects.Count > numHitObjects)
            {
                ClearNested();
            }

            if (numHitObjects == 1)
                return;

            while (NestedHitObjects.Count < numHitObjects)
            {
                AddNested(new StreamHitCircle());
            }

            for (int i = 0; i < numHitObjects; i++)
            {
                float t = (float)i / (numHitObjects - 1);

                var hitObject = (StreamHitCircle)NestedHitObjects[i];

                hitObject.IndexInCurrentCombo = IndexInCurrentCombo + i;
                hitObject.Position = Position + Path.PositionAt(t);
                hitObject.StartTime = StartTime + DurationPerCircle * i;
                hitObject.Scale = Scale;
            }
        }
    }
}
