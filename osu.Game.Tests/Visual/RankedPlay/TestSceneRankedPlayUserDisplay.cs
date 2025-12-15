// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public class TestSceneRankedPlayUserDisplay : OsuTestScene
    {
        private readonly RankedPlayUserDisplay userDisplay;

        public TestSceneRankedPlayUserDisplay()
        {
            Child = userDisplay = new RankedPlayUserDisplay(new APIUser(), Anchor.BottomLeft, RankedPlayColourScheme.Blue)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(256, 72)
            };
        }

        [Test]
        public void TesUserDisplay()
        {
            AddSliderStep("health", 0, 1_000_000, 1_000_000, value => userDisplay.Health.Value = value);
        }
    }
}
