// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Tests.Visual.Multiplayer;
using osuTK;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public class TestSceneMatchmakingBeatmapList : MultiplayerTestScene
    {
        private List<MultiplayerPlaylistItem> selectedBeatmaps = null!;
        private MatchmakingBeatmapList list = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            AddStep("add list", () =>
            {
                selectedBeatmaps = new List<MultiplayerPlaylistItem>();
                MultiplayerPlaylistItem[] beatmaps = Enumerable.Range(1, 50).Select(i => new MultiplayerPlaylistItem
                {
                    BeatmapID = i,
                    StarRating = i / 10.0,
                }).ToArray();

                Child = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(700, 500),
                    Child = list = new MatchmakingBeatmapList(beatmaps)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        SelectionRequested = onSelectionRequested
                    }
                };
            });
        }

        private void onSelectionRequested(MultiplayerPlaylistItem item)
        {
            if (selectedBeatmaps.Remove(item))
            {
                list.RemoveSelection(item, new MultiplayerRoomUser(1)
                {
                    User = new APIUser { Id = 1 }
                });
            }
            else
            {
                selectedBeatmaps.Add(item);
                list.AddSelection(item, new MultiplayerRoomUser(1)
                {
                    User = new APIUser { Id = 1 }
                });
            }
        }
    }
}
