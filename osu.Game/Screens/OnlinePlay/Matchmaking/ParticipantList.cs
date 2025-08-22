// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Idle;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking
{
    public partial class ParticipantList : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private readonly OffsettableFillFlowContainer panels;

        private Drawable? target;

        public ParticipantList()
        {
            InternalChild = panels = new OffsettableFillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Spacing = new Vector2(20, 5),
                LayoutEasing = Easing.OutPow10,
                LayoutDuration = 1000,
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
            panels.Add(new PlayerPanel(user)
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
                if (matchmakingState.Users.UserDictionary.TryGetValue(panel.User.UserID, out MatchmakingUser? user))
                    panels.SetLayoutPosition(panel, user.Placement);
                else
                    panels.SetLayoutPosition(panel, float.MaxValue);
            }
        });

        public void SetTarget(Drawable? target)
        {
            this.target = target;
        }

        protected override void Update()
        {
            base.Update();

            if (target == null)
            {
                panels.Offset = Vector2.Zero;
                panels.Size = Vector2.One;
            }
            else
            {
                var drawQuad = ToLocalSpace(target.ScreenSpaceDrawQuad);

                panels.Offset = drawQuad.TopLeft;
                panels.Size = Vector2.Divide(drawQuad.BottomRight - drawQuad.TopLeft, ChildSize);
            }
        }

        private partial class OffsettableFillFlowContainer : FillFlowContainer<PlayerPanel>
        {
            private Vector2 offset;

            public Vector2 Offset
            {
                get => offset;
                set
                {
                    if (offset == value)
                        return;

                    offset = value;
                    InvalidateLayout();
                }
            }

            public override Vector2 Size
            {
                get => base.Size;
                set
                {
                    if (Precision.AlmostEquals(base.Size, value))
                        return;

                    // Curse you Anchor.Centre (╯°□°)╯
                    Vector2[] anchorPositions = new Vector2[Children.Count];

                    for (int i = 0; i < Children.Count; i++)
                        anchorPositions[i] = Children[i].AnchorPosition;

                    base.Size = value;

                    for (int i = 0; i < Children.Count; i++)
                        Children[i].Position += anchorPositions[i] - Children[i].AnchorPosition;
                }
            }

            protected override IEnumerable<Vector2> ComputeLayoutPositions()
            {
                if (offset != Vector2.Zero)
                    return base.ComputeLayoutPositions().Select(p => p + offset);

                return base.ComputeLayoutPositions();
            }
        }
    }
}
