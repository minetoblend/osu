// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Objects.Types
{
    public interface IHasPath : IHasDistance
    {
        /// <summary>
        /// The curve.
        /// </summary>
        SliderPath Path { get; }
    }
}

public static class HasPathWithExtensions
{
    /// <summary>
    /// Computes the position on the curve relative to how much of the <see cref="HitObject"/> has been completed.
    /// </summary>
    /// <param name="obj">The curve.</param>
    /// <param name="progress">[0, 1] where 0 is the start time of the <see cref="HitObject"/> and 1 is the end time of the <see cref="HitObject"/>.</param>
    /// <returns>The position on the curve.</returns>
    public static Vector2 CurvePositionAt(this IHasPath obj, double progress)
        => obj.Path.PositionAt(progress);
}
