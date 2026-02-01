// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen2 : RankedPlaySubScreen
    {
        private ScoreAnimation scoreAnimation = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children =
            [
                scoreAnimation = new ScoreAnimation(876_349, 834_618, 2f)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            ];
        }

        public override void OnEntering(RankedPlaySubScreen? previous)
        {
            base.OnEntering(previous);

            scoreAnimation.Play();
        }
    }
}
