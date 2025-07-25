// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Screens;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Rooms;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingRoomSubScreen : MultiplayerTestScene
    {
        private MatchmakingRoomSubScreen screen = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("load match", () => LoadScreen(screen = new MatchmakingRoomSubScreen(new Room { Name = "matchmaking" })));
            AddUntilStep("wait for load", () => screen.IsCurrentScreen());
        }
    }
}
