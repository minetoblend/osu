// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Screens.Edit;

namespace osu.Game.Tests.Visual.Editing
{
    public class TestSceneEditorComposeScreen : EditorTestScene
    {
        protected override Ruleset CreateEditorRuleset() => new OsuRuleset();

        [Test]
        public void TestSwitchScreensInstantaneously()
        {
            AddStep("set compose screen", () =>
            {
                Editor.Mode.Value = EditorScreenMode.Compose;
            });
        }
    }
}
