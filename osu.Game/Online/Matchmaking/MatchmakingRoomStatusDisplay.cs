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
                case MatchmakingRoomStatus.WaitingForJoin:
                    text.Text = "Players are joining the room...";
                    break;

                case MatchmakingRoomStatus.WaitForReturn:
                    text.Text = "Players are viewing the results...";
                    break;

                case MatchmakingRoomStatus.WaitForNextRound:
                    text.Text = "Taking a short break before the next round...";
                    break;

                case MatchmakingRoomStatus.WaitForSelection:
                    text.Text = "The next beatmap is being selected...";
                    break;

                case MatchmakingRoomStatus.WaitForStart:
                    text.Text = "The next round is starting!";
                    break;

                case MatchmakingRoomStatus.Pick:
                    text.Text = "Select your beatmap!";
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
