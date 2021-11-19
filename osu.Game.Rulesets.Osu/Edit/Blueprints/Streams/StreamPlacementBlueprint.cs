// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Streams
{
    public class StreamPlacementBlueprint : PlacementBlueprint
    {
        public StreamPlacementBlueprint()
            : base(new Objects.StreamHitObject())
        {
        }
    }
}
