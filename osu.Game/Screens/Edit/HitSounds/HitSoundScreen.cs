// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;

namespace osu.Game.Screens.Edit.HitSounds
{
    public partial class HitSoundScreen : EditorScreenWithTimeline
    {
        private Ruleset ruleset = null!;

        public HitSoundScreen()
            : base(EditorScreenMode.HitSounds)
        {
        }

        [BackgroundDependencyLoader]
        private void load(Bindable<WorkingBeatmap> working)
        {
            ruleset = working.Value.BeatmapInfo.Ruleset.CreateInstance();
        }

        protected override Drawable CreateMainContent() => wrapSkinnableContent(new HitSoundComposer());

        private Drawable wrapSkinnableContent(Drawable content)
        {
            Debug.Assert(ruleset != null);

            return new EditorSkinProvidingContainer(EditorBeatmap).WithChild(content);
        }
    }
}
