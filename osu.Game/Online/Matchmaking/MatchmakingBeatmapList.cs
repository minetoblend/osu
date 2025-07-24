// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osuTK;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingBeatmapList : CompositeDrawable
    {
        public Action<APIBeatmap>? SelectionRequested;

        private readonly APIBeatmap[] beatmaps;

        private FillFlowContainer<MatchmakingBeatmapPanel> panels = null!;

        public MatchmakingBeatmapList(APIBeatmap[] beatmaps)
        {
            this.beatmaps = beatmaps;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new OsuScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = panels = new FillFlowContainer<MatchmakingBeatmapPanel>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(20, 20)
                }
            };

            foreach (var beatmap in beatmaps)
            {
                panels.Add(new MatchmakingBeatmapPanel(beatmap)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    SelectionRequested = b => SelectionRequested?.Invoke(b)
                });
            }
        }

        public void AddSelection(APIBeatmap beatmap, MultiplayerRoomUser user)
        {
            panels.Single(b => b.Beatmap.Equals(beatmap)).AddSelection(user);
        }

        public void RemoveSelection(APIBeatmap beatmap, MultiplayerRoomUser user)
        {
            panels.Single(b => b.Beatmap.Equals(beatmap)).RemoveSelection(user);
        }
    }
}
