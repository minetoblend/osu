// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Scoring;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        public partial class ScoreDetails(ScoreInfo score, RankedPlayColourScheme colourScheme) : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load()
            {
            }
        }
    }
}
