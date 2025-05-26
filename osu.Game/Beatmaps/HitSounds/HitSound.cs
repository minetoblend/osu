// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Beatmaps.HitSounds
{
    public class HitSound : HitObject
    {
        public readonly Bindable<int> LayerBindable = new Bindable<int>();

        public int Layer
        {
            get => LayerBindable.Value;
            set => LayerBindable.Value = value;
        }
    }
}
