// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osuTK;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingPlayerList : CompositeDrawable
    {
        private readonly IList<MultiplayerRoomUser> users;

        private FillFlowContainer<MatchmakingPlayerPanel> panels = null!;

        public MatchmakingPlayerList(IList<MultiplayerRoomUser> users)
        {
            this.users = users;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = panels = new FillFlowContainer<MatchmakingPlayerPanel>
            {
                RelativeSizeAxes = Axes.Both,
                Spacing = new Vector2(20, 5),
                LayoutEasing = Easing.InOutQuint,
                LayoutDuration = 500
            };

            foreach (var user in users)
            {
                panels.Add(new MatchmakingPlayerPanel(user)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                });
            }
        }

        public void ApplyScoreChanges(params MatchmakingScoreChange[] changes)
        {
            foreach (var change in changes)
            {
                MatchmakingPlayerPanel panel = panels.Single(u => u.User.UserID == change.UserId);

                panel.Score = change.Score;
                panel.Rank = change.Rank;

                panels.SetLayoutPosition(panel, panel.Rank);
            }
        }
    }
}
