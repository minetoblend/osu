// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingBeatmapList : CompositeDrawable
    {
        public Action<MultiplayerPlaylistItem>? SelectionRequested;

        private readonly MultiplayerPlaylistItem[] playlist;

        private FillFlowContainer<MatchmakingBeatmapPanel> panels = null!;

        public MatchmakingBeatmapList(MultiplayerPlaylistItem[] playlist)
        {
            this.playlist = playlist;
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

            foreach (MultiplayerPlaylistItem item in playlist)
            {
                var panel = new MatchmakingBeatmapPanel(item)
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    SelectionRequested = i => SelectionRequested?.Invoke(i)
                };

                panels.Add(panel);
                panels.SetLayoutPosition(panel, (float)item.StarRating);
            }
        }

        public void AddSelection(MultiplayerPlaylistItem item, MultiplayerRoomUser user)
        {
            panels.Single(b => b.Item.Equals(item)).AddSelection(user);
        }

        public void RemoveSelection(MultiplayerPlaylistItem item, MultiplayerRoomUser user)
        {
            panels.Single(b => b.Item.Equals(item)).RemoveSelection(user);
        }
    }
}
