// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Rooms;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingSelectionCarousel : MultiplayerTestScene
    {
        private MatchmakingSelectionCarousel carousel = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add carousel", () => Child = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(700, 500),
                Child = carousel = new MatchmakingSelectionCarousel()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both
                }
            });
        }

        [Test]
        public void TestScrollManyItems()
        {
            AddStep("scroll", () =>
            {
                MultiplayerPlaylistItem[] beatmaps = Enumerable.Range(1, 50).Select(i => new MultiplayerPlaylistItem
                {
                    ID = i,
                    BeatmapID = i,
                    StarRating = i / 10.0,
                }).ToArray();

                carousel.BeginScroll(beatmaps, beatmaps[0]);
            });
        }

        [Test]
        public void TestScrollFewItems()
        {
            AddStep("scroll", () =>
            {
                MultiplayerPlaylistItem[] beatmaps = Enumerable.Range(1, 3).Select(i => new MultiplayerPlaylistItem
                {
                    ID = i,
                    BeatmapID = i,
                    StarRating = i / 10.0,
                }).ToArray();

                carousel.BeginScroll(beatmaps, beatmaps[0]);
            });
        }

        [Test]
        public void TestScrollOneItem()
        {
            AddStep("scroll", () =>
            {
                MultiplayerPlaylistItem[] beatmaps = Enumerable.Range(1, 1).Select(i => new MultiplayerPlaylistItem
                {
                    ID = i,
                    BeatmapID = i,
                    StarRating = i / 10.0,
                }).ToArray();

                carousel.BeginScroll(beatmaps, beatmaps[0]);
            });
        }
    }
}
