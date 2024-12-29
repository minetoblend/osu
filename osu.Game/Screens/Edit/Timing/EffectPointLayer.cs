// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Screens.Edit.Timing.Blueprints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class EffectPointLayer : ControlPointLayer<EffectControlPoint, ControlPointBlueprint>
    {
        public EffectPointLayer()
            : base("Effects")
        {
        }

        protected override IReadOnlyList<EffectControlPoint> GetControlPointList(ControlPointInfo controlPointInfo)
            => controlPointInfo.EffectPoints;
    }
}
