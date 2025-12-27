// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Tests.Visual.Matchmaking;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneIntroScreen : MatchmakingTestScene
    {
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("join room", () => JoinRoom(CreateDefaultRoom(MatchType.RankedPlay)));
            WaitForJoined();

            AddStep("join other user", () => MultiplayerClient.AddUser(new APIUser { Id = 2 }));

            AddStep("Add screen", () => Child = new IntroScreen());
        }
    }
}
