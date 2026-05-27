// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Snapping
{
    public record SnapResult(Vector2 Position, Vector2 Origin)
    {
        public Vector2 Delta => Position - Origin;
    }
}
