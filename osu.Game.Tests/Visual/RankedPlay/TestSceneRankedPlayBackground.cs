// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayBackground : OsuTestScene
    {
        public TestSceneRankedPlayBackground()
        {
            Child = new RankedPlayBackground { RelativeSizeAxes = Axes.Both };
        }
    }
}
