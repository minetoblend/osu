// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Screens.OnlinePlay.Matchmaking;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneStageDisplay : MultiplayerTestScene
    {
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("join room", () => JoinRoom(CreateDefaultRoom()));
            WaitForJoined();

            AddStep("add bubble", () => Child = new StageDisplay
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 0.5f,
            });
        }

        [Test]
        public void TestStartCountdown()
        {
            foreach (var status in Enum.GetValues<MatchmakingRoomStatus>())
            {
                AddStep($"{status}", () =>
                {
                    MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
                    {
                        RoomStatus = status
                    }).WaitSafely();

                    MultiplayerClient.StartCountdown(new MatchmakingStatusCountdown
                    {
                        Status = status,
                        TimeRemaining = TimeSpan.FromSeconds(5)
                    }).WaitSafely();
                });

                AddWaitStep("wait a bit", 10);
            }
        }
    }
}
