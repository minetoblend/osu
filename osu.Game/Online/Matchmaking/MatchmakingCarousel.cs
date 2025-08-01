// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Online.Rooms;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingCarousel : CompositeDrawable
    {
        public Action<MultiplayerPlaylistItem>? SelectionRequested;

        private readonly MultiplayerRoomUser[] users;
        private readonly MultiplayerPlaylistItem[] playlist;

        private OsuScrollContainer scroll = null!;
        private MatchmakingPlayerList playerList = null!;
        private MatchmakingBeatmapList beatmapList = null!;

        public MatchmakingCarousel(MultiplayerRoomUser[] users, MultiplayerPlaylistItem[] playlist)
        {
            this.users = users;
            this.playlist = playlist;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = scroll = new NoUserScrollContainer(Direction.Horizontal)
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false,
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new WidthReferenceContainer(() => scroll)
                        {
                            RelativeSizeAxes = Axes.Y,
                            Child = playerList = new MatchmakingPlayerList(users)
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        },
                        new WidthReferenceContainer(() => scroll)
                        {
                            RelativeSizeAxes = Axes.Y,
                            Child = beatmapList = new MatchmakingBeatmapList(playlist)
                            {
                                RelativeSizeAxes = Axes.Both,
                                SelectionRequested = item => SelectionRequested?.Invoke(item)
                            }
                        }
                    }
                }
            };
        }

        public void SetStatus(MatchmakingRoomStatus status)
        {
            switch (status)
            {
                case MatchmakingRoomStatus.WaitForReturn:
                case MatchmakingRoomStatus.WaitForNextRound:
                    scroll.ScrollTo(playerList);
                    break;

                case MatchmakingRoomStatus.Pick:
                    scroll.ScrollTo(beatmapList);
                    break;

                case MatchmakingRoomStatus.WaitForSelection:
                    // Todo:
                    break;
            }
        }

        public void ApplyScoreChanges(params MatchmakingScoreChange[] changes)
            => playerList.ApplyScoreChanges(changes);

        public void AddSelection(MultiplayerPlaylistItem item, MultiplayerRoomUser user)
            => beatmapList.AddSelection(item, user);

        public void RemoveSelection(MultiplayerPlaylistItem item, MultiplayerRoomUser user)
            => beatmapList.RemoveSelection(item, user);

        private class NoUserScrollContainer : OsuScrollContainer
        {
            public NoUserScrollContainer(Direction direction)
                : base(direction)
            {
            }

            protected override void OnUserScroll(double value, bool animated = true, double? distanceDecay = null)
            {
            }

            protected override bool OnClick(ClickEvent e) => false;

            protected override bool OnMouseDown(MouseDownEvent e) => false;

            protected override bool OnDragStart(DragStartEvent e) => false;

            protected override void OnDrag(DragEvent e)
            {
            }

            protected override void OnDragEnd(DragEndEvent e)
            {
            }
        }

        private class WidthReferenceContainer : Container
        {
            private readonly Func<Drawable> reference;

            public WidthReferenceContainer(Func<Drawable> reference)
            {
                this.reference = reference;
            }

            protected override void Update()
            {
                base.Update();
                Width = reference().DrawWidth;
            }
        }
    }
}
