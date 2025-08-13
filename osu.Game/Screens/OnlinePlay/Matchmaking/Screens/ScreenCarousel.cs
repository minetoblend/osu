// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Online.Rooms;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Idle;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Pick;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Results;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Selection;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Screens
{
    public class ScreenCarousel : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private PanelScrollContainer scroll = null!;
        private IdleScreen idleScreen = null!;
        private PickScreen pickScreen = null!;
        private SelectionScreen selectionScreen = null!;
        private ResultsScreen resultsScreen = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = scroll = new PanelScrollContainer(Direction.Horizontal)
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
                            Child = idleScreen = new IdleScreen
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        },
                        new WidthReferenceContainer(() => scroll)
                        {
                            RelativeSizeAxes = Axes.Y,
                            Child = pickScreen = new PickScreen
                            {
                                RelativeSizeAxes = Axes.Both,
                            }
                        },
                        new WidthReferenceContainer(() => scroll)
                        {
                            RelativeSizeAxes = Axes.Y,
                            Child = selectionScreen = new SelectionScreen
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        },
                        new WidthReferenceContainer(() => scroll)
                        {
                            RelativeSizeAxes = Axes.Y,
                            Child = resultsScreen = new ResultsScreen
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onMatchRoomStateChanged;
            onMatchRoomStateChanged(client.Room!.MatchState);
        }

        private void onMatchRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState)
                return;

            switch (matchmakingState.RoomStatus)
            {
                case MatchmakingRoomStatus.RoomStart:
                case MatchmakingRoomStatus.RoundStart:
                case MatchmakingRoomStatus.RoundEnd:
                    scroll.ScrollTo(idleScreen);
                    break;

                case MatchmakingRoomStatus.UserPicks:
                    scroll.ScrollTo(pickScreen);
                    break;

                case MatchmakingRoomStatus.SelectBeatmap:
                    scroll.ScrollTo(selectionScreen);

                    MultiplayerPlaylistItem[] candidateItems = matchmakingState.CandidateItems.Select(item => client.Room!.Playlist.Single(i => i.ID == item)).ToArray();
                    MultiplayerPlaylistItem candidateItem = client.Room!.Playlist.Single(i => i.ID == matchmakingState.CandidateItem);

                    selectionScreen.BeginScroll(candidateItems, candidateItem);
                    break;

                case MatchmakingRoomStatus.RoomEnd:
                    scroll.ScrollTo(resultsScreen);
                    break;
            }
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (client.IsNotNull())
                client.MatchRoomStateChanged -= onMatchRoomStateChanged;
        }

        private class PanelScrollContainer : OsuScrollContainer
        {
            private Drawable? scrollTarget;

            public PanelScrollContainer(Direction direction)
                : base(direction)
            {
            }

            public new void ScrollTo(Drawable target, bool animated = true)
            {
                scrollTarget = target;
                base.ScrollTo(target, animated);
            }

            protected override void Update()
            {
                base.Update();

                if (scrollTarget != null)
                    ScrollTo(scrollTarget);
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
