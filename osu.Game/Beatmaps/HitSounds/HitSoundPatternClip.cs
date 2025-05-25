// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;

namespace osu.Game.Beatmaps.HitSounds
{
    public class HitSoundPatternClip
    {
        public readonly HitSoundPattern Pattern;

        public readonly Bindable<double> StartTimeBindable = new Bindable<double>();

        public double StartTime
        {
            get => StartTimeBindable.Value;
            set => StartTimeBindable.Value = value;
        }

        public HitSoundPatternClip(HitSoundPattern pattern)
        {
            Pattern = pattern;
        }
    }
}
