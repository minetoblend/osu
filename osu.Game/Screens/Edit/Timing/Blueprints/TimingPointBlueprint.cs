// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class TimingPointBlueprint : ControlPointBlueprint<TimingControlPoint>
    {
        protected override ControlPointPiece CreateControlPointPiece() => new TimingPointControlPointPiece(this);

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(new BeatLengthAdjustmentPiece(this)
            {
                Depth = 1
            });
        }
    }
}
