// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Framework.Testing;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
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

        public TestSceneMatchmakingScreen()
        {
            Add(new BackButton
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                State = { Value = Visibility.Visible }
            });
        }

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

                var beatmaps = Enumerable.Range(1, beatmap_count).Select(i => new MultiplayerPlaylistItem
                {
                    BeatmapID = i,
                    StarRating = i / 10.0
                }).ToArray();

                LoadScreen(screen = new MatchmakingScreen(new MultiplayerRoom(0)
                {
                    Users = users,
                    Playlist = beatmaps
                }));
            });
            AddUntilStep("wait for load", () => screen.IsCurrentScreen());
        }

        [Test]
        public void TestGameplayFlow()
        {
            AddWaitStep("wait", 5);

            // Initial "ready" status of the room".

            AddStep("wait for next round", () => MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
            {
                RoomStatus = MatchmakingRoomStatus.WaitForNextRound
            }).WaitSafely());
            AddWaitStep("wait", 5);

            // Next round starts with picks.

            AddStep("pick", () => MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
            {
                RoomStatus = MatchmakingRoomStatus.Pick
            }).WaitSafely());
            AddWaitStep("wait", 5);

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

            AddStep("wait for selection", () =>
            {
                MultiplayerPlaylistItem[] beatmaps = Enumerable.Range(1, 50).Select(i => new MultiplayerPlaylistItem
                {
                    ID = i,
                    BeatmapID = i,
                    StarRating = i / 10.0,
                }).ToArray();

                MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
                {
                    RoomStatus = MatchmakingRoomStatus.WaitForSelection,
                    CandidateItems = beatmaps,
                    GameplayItem = beatmaps[0]
                }).WaitSafely();
            });

            AddWaitStep("wait", 25);

            // Prepare gameplay.

            AddStep("wait for start", () => MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
            {
                RoomStatus = MatchmakingRoomStatus.WaitForStart
            }).WaitSafely());
            AddWaitStep("wait", 5);

            // Start gameplay.

            AddStep("start gameplay", () => MultiplayerClient.StartMatch().WaitSafely());
            // AddUntilStep("wait for player", () => (Stack.CurrentScreen as Player)?.IsLoaded == true);
            AddWaitStep("wait", 5);

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

            AddStep("wait for return", () => MultiplayerClient.ChangeMatchRoomState(new MatchmakingRoomState
            {
                RoomStatus = MatchmakingRoomStatus.WaitForReturn
            }).WaitSafely());
        }
    }
}
