// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Interactions;
using osu.Game.Rulesets.Osu;
using osu.Game.Tests.Beatmaps;
using osuTK;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Editing
{
    [TestFixture]
    public partial class TestSceneComposeInteraction : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new TestBeatmap(ruleset, false);

        [Resolved]
        private FrameworkConfigManager frameworkConfig { get; set; }

        [Test]
        public void TestComposeInteraction()
        {
            AddStep("Begin interaction", () => Editor.ChildrenOfType<HitObjectComposer>().First().BeginInteraction(new TestInteraction()));
        }

        private partial class TestInteraction : ComposeInteraction
        {
            [BackgroundDependencyLoader]
            private void load()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                RelativeSizeAxes = Axes.None;
                Size = new Vector2(100);

                AddInternal(new Box
                {
                    RelativeSizeAxes = Axes.Both
                });
            }

            protected override bool OnDragStart(DragStartEvent e)
            {
                return true;
            }

            protected override void OnDrag(DragEvent e)
            {
                Position += ToParentSpace(e.MousePosition) - ToParentSpace(e.LastMousePosition);
            }

            protected override bool OnMouseDown(MouseDownEvent e) => e.Button == MouseButton.Left;
        }
    }
}
