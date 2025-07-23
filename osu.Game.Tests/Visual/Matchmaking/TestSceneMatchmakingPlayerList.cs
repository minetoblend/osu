// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingPlayerList : MultiplayerTestScene
    {
        private const int user_count = 8;

        private (MultiplayerRoomUser user, int score)[] userScores = null!;
        private MatchmakingPlayerList list = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add list", () =>
            {
                userScores = Enumerable.Range(1, user_count).Select(i =>
                {
                    var user = new MultiplayerRoomUser(i)
                    {
                        User = new APIUser
                        {
                            Username = $"Player {i}"
                        }
                    };

                    return (user, 0);
                }).ToArray();

                Child = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(500, 500),
                    Child = list = new MatchmakingPlayerList(userScores.Select(u => u.user).ToArray())
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                };
            });
        }

        [Test]
        public void TestRandomChanges()
        {
            AddStep("apply random changes", () =>
            {
                MatchmakingScoreChange[] changes = new MatchmakingScoreChange[userScores.Length];

                int[] deltas = Enumerable.Range(1, userScores.Length).ToArray();
                new Random().Shuffle(deltas);

                for (int i = 0; i < userScores.Length; i++)
                    userScores[i] = (userScores[i].user, userScores[i].score + deltas[i]);
                userScores = userScores.OrderByDescending(u => u.score).ToArray();

                for (int i = 0; i < userScores.Length; i++)
                {
                    changes[i] = new MatchmakingScoreChange
                    {
                        UserId = userScores[i].user.UserID,
                        Rank = i + 1,
                        Score = userScores[i].score
                    };
                }

                list.ApplyScoreChanges(changes);
            });
        }
    }
}
