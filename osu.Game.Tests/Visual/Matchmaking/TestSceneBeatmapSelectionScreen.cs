// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Screens.OnlinePlay.Matchmaking;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public partial class TestSceneBeatmapSelectionScreen : ScreenTestScene
    {
        [Test]
        public void TestBeatmapSelectionScreen()
        {
            BeatmapSelectionScreen screen = null!;

            AddStep("push screen", () => LoadScreen(screen = new BeatmapSelectionScreen()));
            AddStep("hide panels", () => screen.HidePanels(4));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void TestNumberBeatmapsRemaining(int remaining)
        {
            BeatmapSelectionScreen screen = null!;

            AddStep("push screen", () =>
            {
                LoadScreen(screen = new BeatmapSelectionScreen());
                Scheduler.AddDelayed(() => screen.HidePanels(remaining), 250);
            });
        }
    }
}
