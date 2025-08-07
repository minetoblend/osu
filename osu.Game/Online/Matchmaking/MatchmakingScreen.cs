// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Cursor;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Online.Rooms;
using osu.Game.Overlays;
using osu.Game.Rulesets;
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

        [Resolved]
        private BeatmapManager beatmapManager { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesets { get; set; } = null!;

        private readonly MultiplayerRoom room;
        private MatchmakingCarousel carousel = null!;

        public MatchmakingScreen(MultiplayerRoom room)
        {
            this.room = room;

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
                                    new MatchmakingRoomStatusDisplay
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
                                            carousel = new MatchmakingCarousel(room.Users.ToArray())
                                            {
                                                RelativeSizeAxes = Axes.Both
                                            }
                                        }
                                    }
                                ],
                                null,
                                [
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 100,
                                        Padding = new MarginPadding
                                        {
                                            Horizontal = 200,
                                        },
                                        Child = new MatchChatDisplay(new Room(room))
                                        {
                                            RelativeSizeAxes = Axes.Both
                                        }
                                    }
                                ]
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
            client.LoadRequested += onLoadRequested;

            onMatchRoomStateChanged(client.Room!.MatchState);
        }

        private void onMatchRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState)
                return;

            if (matchmakingState.RoomStatus == MatchmakingRoomStatus.SelectBeatmap)
                this.Delay(MatchmakingSelectionCarousel.TOTAL_TRANSFORM_TIME).Schedule(updateGameplayState);
        });

        private void updateGameplayState()
        {
            MultiplayerPlaylistItem item = client.Room!.CurrentPlaylistItem;
            RulesetInfo ruleset = rulesets.GetRuleset(item.RulesetID)!;
            Ruleset rulesetInstance = ruleset.CreateInstance();

            // Update global gameplay state to correspond to the new selection.
            // Retrieve the corresponding local beatmap, since we can't directly use the playlist's beatmap info
            var localBeatmap = beatmapManager.QueryBeatmap($@"{nameof(BeatmapInfo.OnlineID)} == $0 AND {nameof(BeatmapInfo.MD5Hash)} == {nameof(BeatmapInfo.OnlineMD5Hash)}", item.BeatmapID);
            Beatmap.Value = beatmapManager.GetWorkingBeatmap(localBeatmap);
            Ruleset.Value = ruleset;
            Mods.Value = item.RequiredMods.Select(m => m.ToMod(rulesetInstance)).ToArray();

            // TODO: Very temporary!
            client.ChangeBeatmapAvailability(BeatmapAvailability.LocallyAvailable()).FireAndForget();
        }

        private void onLoadRequested() => Scheduler.Add(() =>
        {
            updateGameplayState();
            this.Push(new MultiplayerPlayerLoader(() => new MultiplayerPlayer(new Room(room), new PlaylistItem(client.Room!.CurrentPlaylistItem), room.Users.ToArray())));
        });

        public override bool OnExiting(ScreenExitEvent e)
        {
            if (base.OnExiting(e))
                return true;

            client.LeaveRoom();
            return false;
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);

            if (e.Last is not MultiplayerPlayerLoader playerLoader)
                return;

            if (!playerLoader.GameplayPassed)
            {
                client.AbortGameplay().FireAndForget();
                return;
            }

            client.ChangeState(MultiplayerUserState.Idle);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (client.IsNotNull())
            {
                client.MatchRoomStateChanged -= onMatchRoomStateChanged;
                client.LoadRequested -= onLoadRequested;
            }
        }
    }
}
