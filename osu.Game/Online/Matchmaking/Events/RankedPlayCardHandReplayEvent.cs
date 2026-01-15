// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using MessagePack;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;

namespace osu.Game.Online.Matchmaking.Events
{
    [Serializable]
    [MessagePackObject]
    public class RankedPlayCardHandReplayEvent : MatchServerEvent
    {
        /// <summary>
        /// The user performing the action.
        /// </summary>
        [Key(0)]
        public int UserId { get; set; }

        [Key(0)]
        public required Dictionary<Guid, CardHand.CardState> State { get; init; }
    }
}
