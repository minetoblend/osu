// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osu.Game.Graphics.Cursor;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osu.Game.Overlays;
using osu.Game.Screens;
using osu.Game.Screens.OnlinePlay;
using osu.Game.Screens.OnlinePlay.Match.Components;
using osu.Game.Screens.OnlinePlay.Multiplayer;
using osu.Game.Users;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingScreen : OsuScreen
    {
        /// <summary>
        /// Padding between rows of the content.
        /// </summary>
        private const float row_padding = 10;

        public override bool? ApplyModTrackAdjustments => true;

        public override bool DisallowExternalBeatmapRulesetChanges => true;

        [Cached(typeof(OnlinePlayBeatmapAvailabilityTracker))]
        private readonly OnlinePlayBeatmapAvailabilityTracker beatmapAvailabilityTracker = new MultiplayerBeatmapAvailabilityTracker();

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private readonly Room room;
        private readonly MultiplayerRoomUser[] users;
        private readonly APIBeatmap[] beatmaps;

        private readonly List<APIBeatmap> selectedBeatmaps = new List<APIBeatmap>();

        private MatchmakingRoomStatusDisplay statusDisplay = null!;
        private MatchmakingCarousel carousel = null!;

        public MatchmakingScreen(Room room, MultiplayerRoomUser[] users, APIBeatmap[] beatmaps)
        {
            this.room = room;
            this.users = users;
            this.beatmaps = beatmaps;

            Activity.Value = new UserActivity.InLobby(room);
            Padding = new MarginPadding { Horizontal = -HORIZONTAL_OVERFLOW_PADDING };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new OsuContextMenuContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new PopoverContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        beatmapAvailabilityTracker,
                        new MultiplayerRoomSounds(),
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding
                            {
                                Horizontal = WaveOverlayContainer.WIDTH_PADDING,
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize),
                                new Dimension(GridSizeMode.Absolute, row_padding),
                                new Dimension(),
                                new Dimension(GridSizeMode.Absolute, row_padding),
                                new Dimension(GridSizeMode.AutoSize),
                            },
                            Content = new Drawable[]?[]
                            {
                                [
                                    statusDisplay = new MatchmakingRoomStatusDisplay
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                    }
                                ],
                                null,
                                [
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        CornerRadius = 10,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Color4Extensions.FromHex(@"3e3a44") // Temporary.
                                            },
                                            carousel = new MatchmakingCarousel(users, beatmaps)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                SelectionRequested = onBeatmapSelectionRequested
                                            }
                                        }
                                    }
                                ],
                                null,
                                [
                                    new MatchChatDisplay(room)
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 100
                                    }
                                ]
                            }
                        }
                    }
                }
            };
        }

        public void SetStatus(MatchmakingRoomStatus status)
        {
            statusDisplay.Status.Value = status;
            carousel.SetStatus(status);

            switch (status)
            {
                case MatchmakingRoomStatus.InGameplay:
                    this.Push(new MultiplayerPlayerLoader(() => new MultiplayerPlayer(room, new PlaylistItem(client.Room!.CurrentPlaylistItem), users)));
                    break;
            }
        }

        public void ApplyScoreChanges(params MatchmakingScoreChange[] changes)
            => carousel.ApplyScoreChanges(changes);

        private void onBeatmapSelectionRequested(APIBeatmap beatmap)
        {
            if (selectedBeatmaps.Remove(beatmap))
                carousel.RemoveSelection(beatmap, client.LocalUser!);
            else
            {
                selectedBeatmaps.Add(beatmap);
                carousel.AddSelection(beatmap, client.LocalUser!);
            }
        }
    }
}
