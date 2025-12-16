// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayCornerPiece : OsuTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Children =
            [
                new RankedPlayCornerPiece(RankedPlayColourScheme.Blue, Anchor.BottomLeft)
                {
                    Child = new RankedPlayUserDisplay(new APIUser
                    {
                        Username = "BTMC",
                        Id = 3171691,
                    }, Anchor.BottomLeft, RankedPlayColourScheme.Blue)
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
                new RankedPlayCornerPiece(RankedPlayColourScheme.Red, Anchor.TopRight)
                {
                    Child = new RankedPlayUserDisplay(new APIUser
                    {
                        Username = "Happy_24",
                        Id = 12876323,
                    }, Anchor.TopRight, RankedPlayColourScheme.Red)
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
            ];
        }
    }
}
