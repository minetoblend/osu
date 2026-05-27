// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Osu.Objects;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public static class EditorExtensions
    {
        public static IEnumerable<Vector2> GetSnapPositions(this OsuHitObject hitObject) =>
            hitObject switch
            {
                HitCircle => [hitObject.Position],
                Slider slider => [hitObject.Position, hitObject.Position + slider.Path.PositionAt(1)],
                _ => []
            };
    }
}
