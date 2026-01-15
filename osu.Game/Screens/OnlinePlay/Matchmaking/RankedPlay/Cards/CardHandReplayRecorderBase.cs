// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public abstract partial class CardHandReplayRecorderBase : Component
    {
        public double FlushInterval { get; init; } = 1000;

        public double RecordInterval { get; init; } = 25;

        private readonly PlayerCardHand cardHand;

        private readonly List<CardHandReplayFrame> buffer = new List<CardHandReplayFrame>();
        private bool hasChanges;
        private double? lastFrameTime;

        protected CardHandReplayRecorderBase(PlayerCardHand cardHand)
        {
            this.cardHand = cardHand;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Scheduler.AddDelayed(recordFrame, RecordInterval, true);
            Scheduler.AddDelayed(tryFlush, FlushInterval, true);

            cardHand.StateChanged += onHandStateChanged;
        }

        private void onHandStateChanged() => hasChanges = true;

        private void recordFrame()
        {
            if (!hasChanges)
                return;

            double delay = lastFrameTime != null ? Time.Current - lastFrameTime.Value : 0;

            buffer.Add(new CardHandReplayFrame
            {
                Delay = delay,
                Cards = cardHand.State,
            });

            lastFrameTime = Time.Current;
            hasChanges = false;
        }

        private void tryFlush()
        {
            if (buffer.Count == 0)
                return;

            var frames = buffer.ToArray();
            buffer.Clear();

            if (frames.Length > 0)
                Flush(frames);
        }

        protected abstract void Flush(CardHandReplayFrame[] frames);

        protected override void Dispose(bool isDisposing)
        {
            cardHand.StateChanged -= onHandStateChanged;

            base.Dispose(isDisposing);
        }
    }
}
