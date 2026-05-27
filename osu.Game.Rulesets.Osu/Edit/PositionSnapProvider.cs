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

namespace osu.Game.Rulesets.Osu.Edit
{
    public class PositionSnapProvider : Component
    {
        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        public Vector2? SnapToHitObjects(Vector2 playfieldPosition, IEnumerable<OsuHitObject>? exclude = null)
        {
            const float threshold = 3;

            var snapPoints = editorBeatmap.HitObjects
                                          .Cast<OsuHitObject>()
                                          .Except(exclude ?? [])
                                          .Where(isVisible)
                                          .SelectMany(static h => h.GetSnapPositions());

            foreach (var p in snapPoints)
            {
                if (Vector2.Distance(playfieldPosition, p) < threshold)
                    return p;
            }

            return null;
        }

        private bool isVisible(OsuHitObject hitObject)
        {
            return editorClock.CurrentTime >= hitObject.StartTime - hitObject.TimePreempt &&
                   editorClock.CurrentTime < hitObject.GetEndTime() + 800;
        }
    }
}
