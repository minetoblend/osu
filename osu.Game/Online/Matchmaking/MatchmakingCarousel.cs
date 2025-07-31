// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingCarousel : CompositeDrawable
    {
        public Action<APIBeatmap>? SelectionRequested;

        private readonly MultiplayerRoomUser[] users;
        private readonly APIBeatmap[] beatmaps;

        private OsuScrollContainer scroll = null!;
        private MatchmakingPlayerList playerList = null!;
        private MatchmakingBeatmapList beatmapList = null!;

        public MatchmakingCarousel(MultiplayerRoomUser[] users, APIBeatmap[] beatmaps)
        {
            this.users = users;
            this.beatmaps = beatmaps;
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
                            Child = beatmapList = new MatchmakingBeatmapList(beatmaps)
                            {
                                RelativeSizeAxes = Axes.Both,
                                SelectionRequested = b => SelectionRequested?.Invoke(b)
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

        public void AddSelection(APIBeatmap beatmap, MultiplayerRoomUser user)
            => beatmapList.AddSelection(beatmap, user);

        public void RemoveSelection(APIBeatmap beatmap, MultiplayerRoomUser user)
            => beatmapList.RemoveSelection(beatmap, user);

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
