// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using MessagePack;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;

namespace osu.Game.Online.RankedPlay
{
    [Serializable]
    [MessagePackObject]
    public class RankedPlayDiscardResponse
    {
        /// <summary>
        /// The cards that were discarded.
        /// </summary>
        [Key(0)]
        public RankedPlayCardItem[] Discarded { get; set; } = [];

        /// <summary>
        /// Any new cards that were drawn to replace the discarded ones.
        /// </summary>
        [Key(1)]
        public RankedPlayCardItem[] Drawn { get; set; } = [];
    }
}
