// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayCornerPiece : MultiplayerTestScene
    {
        private readonly Bindable<Visibility> visibility = new Bindable<Visibility>(Visibility.Visible);

        [Test]
        public void TestCornerPieces()
        {
            AddStep("add children", () => Children =
            [
                new RankedPlayCornerPiece(RankedPlayColourScheme.Blue, Anchor.BottomLeft)
                {
                    State = { BindTarget = visibility },
                    Child = new RankedPlayUserDisplay(1, Anchor.BottomLeft, RankedPlayColourScheme.Blue)
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
                new RankedPlayCornerPiece(RankedPlayColourScheme.Red, Anchor.TopRight)
                {
                    State = { BindTarget = visibility },
                    Child = new RankedPlayUserDisplay(2, Anchor.TopRight, RankedPlayColourScheme.Red)
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
            ]);

            AddStep("hide", () => visibility.Value = Visibility.Hidden);
            AddStep("show", () => visibility.Value = Visibility.Visible);
        }
    }
}
