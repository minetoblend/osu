// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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

        private FillFlowContainer<MatchmakingPlayerPanel> panels = null!;

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
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onRoomStateChanged;
            client.UserJoined += onUserJoined;
            client.UserLeft += onUserLeft;

            if (client.Room != null)
            {
                onRoomStateChanged(client.Room.MatchState);
                foreach (var user in client.Room.Users)
                    onUserJoined(user);
            }
        }

        private void onUserJoined(MultiplayerRoomUser user) => Scheduler.Add(() =>
        {
            panels.Add(new MatchmakingPlayerPanel(user)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        });

        private void onUserLeft(MultiplayerRoomUser user) => Scheduler.Add(() =>
        {
            panels.Single(p => p.User.Equals(user)).Expire();
        });

        private void onRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState)
                return;

            foreach (var panel in panels)
            {
                if (matchmakingState.UserScores.Scores.TryGetValue(panel.User.UserID, out MatchmakingUserScore? userScore))
                    panels.SetLayoutPosition(panel, userScore.Placement);
                else
                    panels.SetLayoutPosition(panel, float.MaxValue);
            }
        });
    }
}
