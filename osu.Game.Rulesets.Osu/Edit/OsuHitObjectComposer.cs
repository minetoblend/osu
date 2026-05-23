// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Osu.Edit
{
    public class OsuHitObjectComposer : HitObjectComposer<OsuHitObject>
    {
        public OsuHitObjectComposer(Ruleset ruleset)
            : base(ruleset) { }

        [BackgroundDependencyLoader]
        private void load()
        {
            LayerBelowRuleset.Add(new OsuEditorGrid());
        }

        protected override DrawableRuleset<OsuHitObject> CreateDrawableRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods) => new DrawableOsuEditorRuleset(Ruleset, beatmap, mods);
    }
}
