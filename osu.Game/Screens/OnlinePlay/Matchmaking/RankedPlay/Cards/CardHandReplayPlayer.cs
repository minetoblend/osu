// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.RankedPlay;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class CardHandReplayPlayer : CardHandReplayPlayerBase
    {
        public CardHandReplayPlayer(OpponentCardHand cardHand)
            : base(cardHand)
        {
        }

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchEvent += onMatchEvent;
        }

        private void onMatchEvent(MatchServerEvent e)
        {
            if (e is not RankedPlayCardHandReplayEvent replayEvent || replayEvent.UserId == client.LocalUser?.UserID)
                return;

            EnqueueFrames(replayEvent.Frames);
        }

        protected override void Dispose(bool isDisposing)
        {
            client.MatchEvent -= onMatchEvent;

            base.Dispose(isDisposing);
        }
    }
}
