// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingStatusDisplay : MultiplayerTestScene
    {
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("create display", () => Child = new MatchmakingRoomStatusDisplay
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }

        [TestCase(MatchmakingRoomStatus.WaitingForJoin)]
        [TestCase(MatchmakingRoomStatus.WaitForReturn)]
        [TestCase(MatchmakingRoomStatus.WaitForNextRound)]
        [TestCase(MatchmakingRoomStatus.Pick)]
        [TestCase(MatchmakingRoomStatus.WaitForSelection)]
        [TestCase(MatchmakingRoomStatus.WaitForStart)]
        public void TestStatus(MatchmakingRoomStatus status)
        {
            AddStep("set status", () => MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState { RoomStatus = status }).WaitSafely());
        }
    }
}
