// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public class TestSceneScoreCounter : OsuTestScene
    {
        private ScoreCounter scoreCounter;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = scoreCounter = new ScoreCounter(7)
            {
                Font = OsuFont.GetFont(size: 40, weight: FontWeight.Bold, fixedWidth: true),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }

        [SetUp]
        public void Setup()
        {
            AddSliderStep<long>("Value", 0, 1_000_000, 0, value => scoreCounter.Value = value);
        }
    }
}
