// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;

namespace osu.Game.Beatmaps.HitSounds
{
    public class HitSoundPattern
    {
        public event Action<HitSound>? HitSoundAdded;

        public event Action<HitSound>? HitSoundRemoved;

        public readonly Bindable<string> NameBindable = new Bindable<string>();

        public string Name
        {
            get => NameBindable.Value;
            set => NameBindable.Value = value;
        }

        public readonly BindableList<HitSoundLayer> Layers = new BindableList<HitSoundLayer>();

        private readonly List<HitSound> hitSounds = new List<HitSound>();

        public IList<HitSound> HitSounds
        {
            get => hitSounds;
            init => hitSounds.AddRange(value);
        }

        public void AddRange(IEnumerable<HitSound> hitSound)
        {
            foreach (var h in hitSounds)
                Add(h);
        }

        public void Add(HitSound hitSound)
        {
            hitSounds.Add(hitSound);
            HitSoundAdded?.Invoke(hitSound);
        }

        public void RemoveRange(IEnumerable<HitSound> hitSound)
        {
            foreach (var h in hitSounds)
                Remove(h);
        }

        public bool Remove(HitSound hitSound)
        {
            if (!hitSounds.Remove(hitSound))
                return false;

            HitSoundRemoved?.Invoke(hitSound);

            return true;
        }
    }
}
