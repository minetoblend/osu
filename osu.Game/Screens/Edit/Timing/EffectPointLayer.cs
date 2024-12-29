// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics;
using osu.Game.Screens.Edit.Timing.Blueprints;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class EffectPointLayer : ControlPointLayer<EffectControlPoint, ControlPointBlueprint>
    {
        public EffectPointLayer()
            : base("Effects")
        {
        }

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        public override Color4 LayerColour => colours.Orange1;

        protected override IReadOnlyList<EffectControlPoint> GetControlPointList(ControlPointInfo controlPointInfo)
            => controlPointInfo.EffectPoints;
    }
}
