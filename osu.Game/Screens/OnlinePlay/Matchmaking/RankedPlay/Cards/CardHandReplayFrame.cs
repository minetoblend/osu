// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    [Serializable]
    [MessagePackObject]
    public readonly record struct CardHandReplayFrame
    {
        /// <summary>
        /// Duration in milliseconds since the previous frame.
        /// </summary>
        [Key(0)]
        public required double Delay { get; init; }

        /// <summary>
        /// Dictionary containing the state of each card.
        /// </summary>
        [Key(1)]
        public required Dictionary<Guid, CardHand.CardState> Cards { get; init; }

        /// <summary>
        /// Creates a replay frame that only contains state entries that differ from the previous frame
        /// </summary>
        public CardHandReplayFrame RelativeTo(CardHandReplayFrame other) => this with
        {
            Cards = Cards.Where(entry => !other.Cards.Contains(entry)).ToDictionary(),
        };
    }
}
