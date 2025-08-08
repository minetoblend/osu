// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingRoomStatusDisplay : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private OsuSpriteText text = null!;

        public MatchmakingRoomStatusDisplay()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = text = new OsuSpriteText
            {
                Font = OsuFont.Default.With(size: 24)
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
                    text.Text = "Players are joining the room...";
                    break;

                case MatchmakingRoomStatus.RoundStart:
                    text.Text = "Next round starting shortly...";
                    break;

                case MatchmakingRoomStatus.UserPicks:
                    text.Text = "Select your beatmap!";
                    break;

                case MatchmakingRoomStatus.SelectBeatmap:
                    text.Text = "And the next beatmap is...";
                    break;

                case MatchmakingRoomStatus.PrepareBeatmap:
                    text.Text = "Waiting for players to download the beatmap...";
                    break;

                case MatchmakingRoomStatus.PrepareGameplay:
                    text.Text = "Match is starting shortly...";
                    break;

                case MatchmakingRoomStatus.Gameplay:
                    text.Text = "Match is in progress!";
                    break;

                case MatchmakingRoomStatus.RoundEnd:
                    text.Text = "Players are viewing the results...";
                    break;

                case MatchmakingRoomStatus.RoomEnd:
                    text.Text = "Thanks for playing! Room will close shortly.";
                    break;
            }
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (client.IsNotNull())
                client.MatchRoomStateChanged -= onMatchRoomStateChanged;
        }
    }
}
