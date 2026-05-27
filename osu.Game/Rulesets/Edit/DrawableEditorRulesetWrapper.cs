// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Edit
{
    public class DrawableEditorRulesetWrapper<TObject> : CompositeDrawable
        where TObject : HitObject
    {
        public Playfield Playfield => drawableRuleset.Playfield;

        private readonly DrawableRuleset<TObject> drawableRuleset;

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        public override bool PropagatePositionalInputSubTree => false;

        public override bool PropagateNonPositionalInputSubTree => false;

        public DrawableEditorRulesetWrapper(DrawableRuleset<TObject> drawableRuleset)
        {
            this.drawableRuleset = drawableRuleset;

            RelativeSizeAxes = Axes.Both;

            InternalChild = drawableRuleset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            drawableRuleset.FrameStablePlayback = false;
            Playfield.DisplayJudgements.Value = false;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmap.HitObjectAdded += hitObjectAdded;
            beatmap.HitObjectRemoved += hitObjectRemoved;

            Scheduler.AddOnce(regenerateAutoplay);
        }

        private void regenerateAutoplay()
        {
            var autoplayMod = drawableRuleset.Mods.OfType<ModAutoplay>().Single();
            drawableRuleset.SetReplayScore(autoplayMod.CreateScoreFromReplayData(drawableRuleset.Beatmap, drawableRuleset.Mods));
        }

        private void hitObjectAdded(HitObject hitObject)
        {
            drawableRuleset.AddHitObject((TObject)hitObject);
            drawableRuleset.Playfield.PostProcess();
        }

        private void hitObjectRemoved(HitObject hitObject)
        {
            drawableRuleset.RemoveHitObject((TObject)hitObject);
            drawableRuleset.Playfield.PostProcess();
        }
    }
}
