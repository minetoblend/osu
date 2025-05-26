// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Audio;

namespace osu.Game.Beatmaps.HitSounds
{
    public class HitSoundLayer
    {
        public readonly Bindable<string> NameBindable = new Bindable<string>();

        public string Name
        {
            get => NameBindable.Value;
            set => NameBindable.Value = value;
        }

        public readonly Bindable<HitSampleInfo> SampleInfoBindable = new Bindable<HitSampleInfo>();

        public HitSampleInfo SampleInfo
        {
            get => SampleInfoBindable.Value;
            set => SampleInfoBindable.Value = value;
        }

        public int Index;
    }
}
