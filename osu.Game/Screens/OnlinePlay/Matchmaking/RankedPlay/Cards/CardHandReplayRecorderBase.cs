// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public abstract partial class CardHandReplayRecorderBase : Component
    {
        public double FlushInterval { get; init; } = 1000;

        public double RecordInterval { get; init; } = 25;

        public int MaxBufferSize = 20;

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

            cardHand.StateChanged += handStateChanged;
        }

        private void handStateChanged() => hasChanges = true;

        private void recordFrame()
        {
            if (!hasChanges || buffer.Count >= MaxBufferSize)
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

            var frames = compress(buffer).ToArray();
            buffer.Clear();

            if (frames.Length > 0)
                Flush(frames);
        }

        /// <summary>
        /// Compresses a list of <see cref="CardHandReplayFrame"/>s by only keeping values that have changed between each frame
        /// </summary>
        private IEnumerable<CardHandReplayFrame> compress(IReadOnlyList<CardHandReplayFrame> frames)
        {
            if (frames.Count == 0)
                yield break;

            // The first frame always contains the full state since the replay player may drop frames starting from the end for each message.
            yield return frames[0];

            var lastFrame = frames[0];

            foreach (var frame in frames.Skip(1))
            {
                yield return frame.RelativeTo(lastFrame);

                lastFrame = frame;
            }
        }

        protected abstract void Flush(CardHandReplayFrame[] frames);

        protected override void Dispose(bool isDisposing)
        {
            cardHand.StateChanged -= handStateChanged;

            base.Dispose(isDisposing);
        }
    }
}
