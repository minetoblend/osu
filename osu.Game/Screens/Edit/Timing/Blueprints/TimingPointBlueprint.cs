// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class TimingPointBlueprint : ControlPointBlueprint
    {
        public new TimingControlPoint ControlPoint => (TimingControlPoint)base.ControlPoint;
    }
}
