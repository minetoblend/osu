// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingBeatmapPanel : MultiplayerTestScene
    {
        private MatchmakingBeatmapPanel panel = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add beatmap panel", () =>
            {
                Child = panel = new MatchmakingBeatmapPanel(new MultiplayerPlaylistItem { StarRating = 5.3 })
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                };
            });
        }

        [Test]
        public void TestAddRemoveSelection()
        {
            AddStep("add peppy selection", () => panel.AddSelection(new MultiplayerRoomUser(2)
            {
                User = new APIUser
                {
                    Id = 2,
                    Username = "peppy"
                }
            }));

            AddStep("add flyte selection", () => panel.AddSelection(new MultiplayerRoomUser(3103765)
            {
                User = new APIUser
                {
                    Id = 3103765,
                    Username = "flyte"
                }
            }));

            AddStep("remove peppy selection", () => panel.RemoveSelection(new MultiplayerRoomUser(2)));
            AddStep("remove flyte selection", () => panel.RemoveSelection(new MultiplayerRoomUser(3103765)));
        }
    }
}
