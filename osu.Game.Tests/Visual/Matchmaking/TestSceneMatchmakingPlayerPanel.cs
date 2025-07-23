// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingPlayerPanel : MultiplayerTestScene
    {
        private MatchmakingPlayerPanel panel = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add panel", () => Child = panel = new MatchmakingPlayerPanel(new MultiplayerRoomUser(1)
            {
                User = new APIUser
                {
                    Username = "Player 1",
                }
            })
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }

        [Test]
        public void TestIncreaseRank()
        {
            AddStep("increase rank", () => panel.Rank++);
        }

        [Test]
        public void TestIncreaseScore()
        {
            AddStep("increase score", () => panel.Score++);
        }
    }
}
