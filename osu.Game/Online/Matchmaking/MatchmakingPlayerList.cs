// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osuTK;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingPlayerList : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

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

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onRoomStateChanged;
            onRoomStateChanged(client.Room!.MatchState);
        }

        private void onRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState)
                return;

            foreach (var score in matchmakingState.UserScores.Scores.Values)
            {
                MatchmakingPlayerPanel panel = panels.Single(u => u.User.UserID == score.UserId);
                panels.SetLayoutPosition(panel, score.Placement);
            }
        });
    }
}
