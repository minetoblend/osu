// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    public enum MatchmakingRoomStatus
    {
        /// <summary>
        /// Players are still joining the room.
        /// </summary>
        Joining,

        /// <summary>
        /// Next round is starting.
        /// </summary>
        RoundStart,

        /// <summary>
        /// Players are selecting their beatmaps.
        /// </summary>
        PickBeatmap,

        /// <summary>
        /// A beatmap from the pool is being selected by the server.
        /// </summary>
        Selection,

        /// <summary>
        /// Gameplay is starting shortly.
        /// </summary>
        PrepareGameplay,

        /// <summary>
        /// Gameplay is in progress.
        /// </summary>
        Gameplay,

        /// <summary>
        /// Some players are viewing results.
        /// </summary>
        Results
    }
}
