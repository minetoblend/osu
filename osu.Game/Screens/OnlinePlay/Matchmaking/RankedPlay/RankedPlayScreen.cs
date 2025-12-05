// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen : OsuScreen
    {
        private readonly MultiplayerRoom room;
        private readonly Container subscreenContainer;

        [Cached]
        private readonly PlayerCardHand playerHand;

        public RankedPlayScreen(MultiplayerRoom room)
        {
            this.room = room;

            InternalChildren =
            [
                subscreenContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                },
                playerHand = new PlayerCardHand
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 0.5f,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                }
            ];
        }

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onMatchRoomStateChanged;
            onMatchRoomStateChanged(client.Room!.MatchState);
        }

        private void onMatchRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not RankedPlayRoomState rankedPlayState)
                return;

            switch (rankedPlayState.Stage)
            {
                case RankedPlayStage.CardDiscard:
                    RankedPlayUserState userState = (RankedPlayUserState)client.LocalUser!.MatchState!;

                    subscreenContainer.Child = new DiscardScreen(userState.Hand);
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
