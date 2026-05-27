// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit.Tools;
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

        protected abstract IEnumerable<ComposeToolInfo> Tools { get; }

        protected abstract SelectToolInfo SelectToolInfo { get; }

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Cached(typeof(IBindable<ComposeToolInfo>))]
        private readonly Bindable<ComposeToolInfo> activeTool = new Bindable<ComposeToolInfo>();

        public override void SetActiveTool(ComposeToolInfo toolInfo)
        {
            activeTool.Value = toolInfo;
        }

        public EditorClock EditorClock => editorClock;

        private DependencyContainer dependencies = null!;

        protected readonly Container LayerBelowRuleset = new Container { RelativeSizeAxes = Axes.Both, };

        protected HitObjectComposer(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        protected Container LeftToolbarArea = null!;
        protected Container MainContentArea = null!;

        private ComposeToolbar toolbar = null!;
        private ComposeToolContainer toolContainer = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            activeTool.Value = SelectToolInfo;

            DrawableRuleset = CreateDrawableRuleset(editorBeatmap.PlayableBeatmap, [Ruleset.GetAutoplayMod()!]);
            InternalChildren =
            [
                MainContentArea = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Children =
                    [
                        DrawableRuleset.CreatePlayfieldAdjustmentContainer().WithChild(LayerBelowRuleset),
                        // tool container should run its update look before the drawable ruleset
                        // so the playfield has a chance to run an update loop after changes have been made
                        // to the beatmap
                        toolContainer = new ComposeToolContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        new DrawableEditorRulesetWrapper<TObject>(DrawableRuleset)
                        {
                            Clock = EditorClock,
                            ProcessCustomClock = false,
                        },
                        toolContainer.CreateProxy(),
                    ]
                },
                LeftToolbarArea = new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Padding = new MarginPadding(10),
                    Child = toolbar = new ComposeToolbar(Tools.Prepend(SelectToolInfo)),
                }
            ];

            dependencies.CacheAs(DrawableRuleset);
            dependencies.CacheAs(Playfield);
        }

        protected override void Update()
        {
            base.Update();

            MainContentArea.Width = DrawWidth - LeftToolbarArea.DrawWidth * 2;
        }

        protected virtual DrawableRuleset<TObject> CreateDrawableRuleset(IBeatmap beatmap, IReadOnlyList<Mod> mods) =>
            (DrawableRuleset<TObject>)Ruleset.CreateDrawableRulesetWith(beatmap, mods);

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => DrawableRuleset.CreatePlayfieldAdjustmentContainer();
    }

    [Cached]
    public abstract partial class HitObjectComposer : CompositeDrawable
    {
        public const float TOOLBOX_CONTRACTED_SIZE_LEFT = 60;
        public const float TOOLBOX_CONTRACTED_SIZE_RIGHT = 120;

        public abstract void SetActiveTool(ComposeToolInfo toolInfo);

        public abstract PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer();
    }
}
