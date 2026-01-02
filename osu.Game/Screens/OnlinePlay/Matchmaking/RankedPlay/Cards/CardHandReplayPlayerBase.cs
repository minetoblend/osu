// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public abstract partial class CardHandReplayPlayerBase : Component
    {
        public int MaxQueuedFrames { get; set; } = 20;

        private int queuedFrames;
        private double? lastPlayback;
        private readonly OpponentCardHand cardHand;

        protected CardHandReplayPlayerBase(OpponentCardHand cardHand)
        {
            this.cardHand = cardHand;
        }

        protected void EnqueueFrames(CardHandReplayFrame[] frames)
        {
            foreach (var frame in frames)
            {
                if (queuedFrames >= MaxQueuedFrames)
                    return;

                queuedFrames++;

                double delay = Math.Max(lastPlayback != null ? lastPlayback.Value + frame.Delay - Time.Current : 0, 0);
                lastPlayback = Time.Current + delay;

                Scheduler.AddDelayed(() =>
                {
                    queuedFrames--;

                    cardHand.SetState(frame.Cards);
                }, delay);
            }
        }
    }
}
