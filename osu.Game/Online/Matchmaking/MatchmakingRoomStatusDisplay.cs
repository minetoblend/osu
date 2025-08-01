// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingRoomStatusDisplay : CompositeDrawable
    {
        public readonly Bindable<MatchmakingRoomStatus> Status = new Bindable<MatchmakingRoomStatus>();

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

            Status.BindValueChanged(onStatusChanged, true);
        }

        private void onStatusChanged(ValueChangedEvent<MatchmakingRoomStatus> status)
        {
            switch (status.NewValue)
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
        }
    }
}
