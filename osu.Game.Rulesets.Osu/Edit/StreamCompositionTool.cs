// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Streams;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Edit
{
    public class StreamCompositionTool: HitObjectCompositionTool
    {
        public StreamCompositionTool()
            : base(nameof(Stream))
        {
        }

        public override PlacementBlueprint CreatePlacementBlueprint() => new StreamPlacementBlueprint();

        public override Drawable CreateIcon() => new BeatmapStatisticIcon(BeatmapStatisticsIconType.OverallDifficulty);
    }
}
