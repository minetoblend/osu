// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingScreen : MultiplayerTestScene
    {
        private const int user_count = 8;
        private const int beatmap_count = 50;

        private MultiplayerRoomUser[] users = null!;
        private int[] scores = null!;
        private MatchmakingScreen screen = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("join room", () => JoinRoom(CreateDefaultRoom()));
            WaitForJoined();

            AddStep("load match", () =>
            {
                users = Enumerable.Range(1, user_count).Select(i => new MultiplayerRoomUser(i)
                {
                    User = new APIUser
                    {
                        Username = $"Player {i}"
                    }
                }).ToArray();

                scores = new int[users.Length];

                var beatmaps = Enumerable.Range(1, beatmap_count).Select(i =>
                {
                    var beatmap = CreateAPIBeatmap();
                    beatmap.OnlineID = i;
                    beatmap.StarRating = i / 10.0;
                    beatmap.DifficultyName = $"Beatmap {i}";
                    return beatmap;
                }).ToArray();

                LoadScreen(screen = new MatchmakingScreen(new Room { Name = "matchmaking" }, users, beatmaps));
            });
            AddUntilStep("wait for load", () => screen.IsCurrentScreen());
        }

        [Test]
        public void TestGameplayFlow()
        {
            AddWaitStep("wait", 3);

            // Initial "ready" status of the room".

            AddStep("set joined status", () => screen.SetStatus(MatchmakingRoomStatus.WaitForNextRound));
            AddWaitStep("wait", 3);

            // Next round starts with picks.

            AddStep("request pick", () => screen.SetStatus(MatchmakingRoomStatus.Pick));
            AddWaitStep("wait", 3);

            // Make some selections

            for (int i = 0; i < 3; i++)
            {
                int j = i * 2;
                AddStep("click a beatmap", () =>
                {
                    InputManager.MoveMouseTo(this.ChildrenOfType<MatchmakingBeatmapPanel>().ElementAt(j));
                    InputManager.Click(MouseButton.Left);
                });

                AddWaitStep("wait", 2);
            }

            // Lock in the gameplay beatmap

            // Todo:

            // Prepare gameplay.

            AddStep("set wait for start status", () => screen.SetStatus(MatchmakingRoomStatus.WaitForStart));
            AddWaitStep("wait", 3);

            // Start gameplay.

            AddStep("start gameplay", () => screen.SetStatus(MatchmakingRoomStatus.InGameplay));
            // AddUntilStep("wait for player", () => (Stack.CurrentScreen as Player)?.IsLoaded == true);
            AddWaitStep("wait", 3);

            // Finish gameplay.

            AddStep("add some scores", () =>
            {
                MatchmakingScoreChange[] changes = new MatchmakingScoreChange[users.Length];

                int[] deltas = Enumerable.Range(1, users.Length).ToArray();
                new Random().Shuffle(deltas);
                for (int i = 0; i < users.Length; i++)
                    scores[i] += deltas[i];

                MultiplayerRoomUser[] sortedUsers = users.Select((u, i) => (user: u, index: i)).OrderByDescending(item => scores[item.index]).Select(item => item.user).ToArray();

                for (int i = 0; i < users.Length; i++)
                {
                    changes[i] = new MatchmakingScoreChange
                    {
                        UserId = sortedUsers[i].UserID,
                        Rank = i + 1,
                        Score = scores[i]
                    };
                }

                screen.ApplyScoreChanges(changes);
            });

            AddStep("set wait for return status", () => screen.SetStatus(MatchmakingRoomStatus.WaitForReturn));
        }
    }
}
