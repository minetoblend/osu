// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Rulesets.Scoring;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingResultsPanel : MultiplayerTestScene
    {
        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("join room", () => JoinRoom(CreateDefaultRoom()));
            WaitForJoined();

            AddStep("add results screen", () => Child = new MatchmakingResultsPanel
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f)
            });

            AddStep("join users", () => Enumerable.Range(1, 8).ForEach(u =>
            {
                MultiplayerClient.AddUser(new MultiplayerRoomUser(u)
                {
                    User = new APIUser
                    {
                        Id = u,
                        Username = $"User {u}"
                    }
                });

                MultiplayerClient.AddUser(new MultiplayerRoomUser(1000)
                {
                    User = new APIUser
                    {
                        Id = 1000,
                        Username = "Invalid user"
                    }
                });
            }));
        }

        [Test]
        public void TestResults()
        {
            AddStep("set results stage", () =>
            {
                var state = new MatchmakingRoomState
                {
                    Round = 6,
                    RoomStatus = MatchmakingRoomStatus.RoomEnd
                };

                // Highest score.
                state.Users[1].Rounds[1].TotalScore = 1000;
                state.Users[1000].Rounds[1].TotalScore = 990;

                // Highest accuracy.
                state.Users[2].Rounds[2].Accuracy = 0.9995;
                state.Users[1000].Rounds[2].Accuracy = 0.5;

                // Highest combo.
                state.Users[3].Rounds[3].MaxCombo = 100;
                state.Users[1000].Rounds[3].MaxCombo = 10;

                // Most bonus score.
                state.Users[4].Rounds[4].Statistics[HitResult.LargeBonus] = 50;
                state.Users[1000].Rounds[4].Statistics[HitResult.LargeBonus] = 25;

                // Smallest score difference.
                state.Users[5].Rounds[5].TotalScore = 1000;
                state.Users[1000].Rounds[5].TotalScore = 999;

                // Largest score difference.
                state.Users[6].Rounds[6].TotalScore = 1000;
                state.Users[1000].Rounds[6].TotalScore = 0;

                MultiplayerClient.ChangeMatchRoomState(state).WaitSafely();
            });
        }
    }
}
