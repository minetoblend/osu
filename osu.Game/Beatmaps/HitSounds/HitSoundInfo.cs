// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;

namespace osu.Game.Beatmaps.HitSounds
{
    public class HitSoundInfo
    {
        public readonly BindableList<HitSoundPattern> Patterns = new BindableList<HitSoundPattern>();

        public readonly BindableList<HitSoundPatternClip> Clips = new BindableList<HitSoundPatternClip>();
    }
}
