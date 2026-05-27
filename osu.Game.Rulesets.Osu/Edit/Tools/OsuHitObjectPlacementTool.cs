// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Utils;
using osu.Game.Rulesets.Edit.Tools;
using osu.Game.Rulesets.Osu.Edit.Snapping;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Edit.Tools
{
    public class OsuHitObjectPlacementTool<TObject> : HitObjectPlacementTool<TObject>
        where TObject : OsuHitObject
    {
        protected OsuHitObjectPlacementTool(TObject hitObject)
            : base(hitObject)
        {
        }

        [Resolved]
        private PositionSnapProvider snapProvider { get; set; } = null!;

        protected PositionSnapProvider SnapProvider => snapProvider;

        protected override bool AddToBeatmapImmediately => true;

        protected override void OnBeginPlacement()
        {
            base.OnBeginPlacement();

            var overlapping = EditorBeatmap.HitObjects
                                           .Where(it => it != HitObject && Precision.AlmostEquals(it.StartTime, HitObject.StartTime, 1))
                                           .ToArray();

            foreach (var h in overlapping)
                EditorBeatmap.Remove(h);
        }
    }
}
