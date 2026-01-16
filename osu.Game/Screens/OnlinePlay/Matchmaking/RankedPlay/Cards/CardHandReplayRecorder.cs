// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.RankedPlay;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class CardHandReplayRecorder : CardHandReplayRecorderBase
    {
        public CardHandReplayRecorder(PlayerCardHand cardHand)
            : base(cardHand)
        {
        }

        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected override void Flush(RankedPlayCardHandReplayFrame[] frames)
        {
            if (frames.Length == 0)
                return;

            client.SendMatchRequest(new RankedPlayCardHandReplayRequest
            {
                Frames = frames,
            });
        }
    }
}
