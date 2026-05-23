// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Edit
{
    public abstract partial class HitObjectComposer<TObject> : HitObjectComposer
        where TObject : HitObject
    {
        public readonly Ruleset Ruleset;

        public DrawableRuleset<TObject> DrawableRuleset { get; private set; } = null!;

        public Playfield Playfield => DrawableRuleset.Playfield;

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        public EditorClock EditorClock => editorClock;

        private DependencyContainer dependencies = null!;

        protected readonly Container LayerBelowRuleset = new Container { RelativeSizeAxes = Axes.Both, };

        protected HitObjectComposer(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            DrawableRuleset = CreateDrawableRuleset(editorBeatmap.PlayableBeatmap, [Ruleset.GetAutoplayMod()!]);
            InternalChildren =
            [
                DrawableRuleset.CreatePlayfieldAdjustmentContainer().WithChild(LayerBelowRuleset),
                new DrawableEditorRulesetWrapper<TObject>(DrawableRuleset)
                {
                    Clock = EditorClock,
                    ProcessCustomClock = false,
                },
            ];

            dependencies.CacheAs(DrawableRuleset);
            dependencies.CacheAs(Playfield);
        }

        protected virtual DrawableRuleset<TObject> CreateDrawableRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods) =>
            (DrawableRuleset<TObject>)Ruleset.CreateDrawableRulesetWith(beatmap, mods);

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(parent);
    }

    public abstract partial class HitObjectComposer : CompositeDrawable
    {
        public const float TOOLBOX_CONTRACTED_SIZE_LEFT = 60;
        public const float TOOLBOX_CONTRACTED_SIZE_RIGHT = 120;
    }
}
