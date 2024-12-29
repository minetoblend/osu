// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.


namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public interface IPooledControlPointBlueprintProvider
    {
        ControlPointBlueprint CreateBlueprintFor(ControlPointLifetimeEntry controlPoint);
    }
}
