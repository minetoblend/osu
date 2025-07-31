// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingCarousel : MultiplayerTestScene
    {
        private const int user_count = 8;
        private const int beatmap_count = 50;

        private MatchmakingCarousel carousel = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add carousel", () =>
            {
                var users = Enumerable.Range(1, user_count).Select(i => new MultiplayerRoomUser(i)
                {
                    User = new APIUser
                    {
                        Username = $"Player {i}"
                    }
                }).ToArray();

                var beatmaps = Enumerable.Range(1, beatmap_count).Select(i =>
                {
                    var beatmap = CreateAPIBeatmap();
                    beatmap.OnlineID = i;
                    beatmap.StarRating = i / 10.0;
                    beatmap.DifficultyName = $"Beatmap {i}";
                    return beatmap;
                }).ToArray();

                Child = carousel = new MatchmakingCarousel(users, beatmaps)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.7f)
                };
            });
        }

        [Test]
        public void TestStatus()
        {
            AddWaitStep("wait for scroll", 5);
            AddStep("set pick status", () => carousel.SetStatus(MatchmakingRoomStatus.Pick));
            AddWaitStep("wait for scroll", 5);
            AddStep("set wait for next round status", () => carousel.SetStatus(MatchmakingRoomStatus.WaitForNextRound));
        }
    }
}
