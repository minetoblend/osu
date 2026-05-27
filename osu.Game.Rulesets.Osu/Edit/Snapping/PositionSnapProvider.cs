// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Snapping
{
    public class PositionSnapProvider : Component
    {
        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        public SnapResult? SnapToHitObjects(IEnumerable<Vector2> positions, IEnumerable<OsuHitObject>? exclude = null)
        {
            var snapPoints = editorBeatmap.HitObjects
                                          .Cast<OsuHitObject>()
                                          .Except(exclude ?? [])
                                          .Where(isVisible)
                                          .SelectMany(static h => h.GetSnapPositions());

            return snapToPoints(positions, snapPoints);
        }

        public SnapResult? SnapToHitObjects(Vector2 position, IEnumerable<OsuHitObject>? exclude = null) =>
            SnapToHitObjects([position], exclude);

        private static SnapResult? snapToPoints(IEnumerable<Vector2> sourcePoints, IEnumerable<Vector2> targetPoints, float threshold = 3)
        {
            foreach (var source in sourcePoints)
            {
                foreach (var target in targetPoints)
                {
                    if (Vector2.Distance(source, target) < threshold)
                        return new SnapResult(target, source);
                }
            }

            return null;
        }

        private bool isVisible(OsuHitObject hitObject)
        {
            return editorClock.CurrentTime >= hitObject.StartTime - hitObject.TimePreempt &&
                   editorClock.CurrentTime < hitObject.GetEndTime() + OsuHitObjectComposer.FADEOUT_DURATION;
        }
    }
}
