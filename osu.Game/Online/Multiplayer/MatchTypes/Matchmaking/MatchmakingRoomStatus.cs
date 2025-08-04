// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Multiplayer.MatchTypes.Matchmaking
{
    public enum MatchmakingRoomStatus
    {
        /// <summary>
        /// Waiting for all players to join the room.
        /// </summary>
        WaitingForJoin,

        /// <summary>
        /// Waiting for all players to return from the results screen.
        /// </summary>
        WaitForReturn,

        /// <summary>
        /// Waiting for the next round to begin.
        /// </summary>
        WaitForNextRound,

        /// <summary>
        /// Players are selecting their beatmaps.
        /// </summary>
        Pick,

        /// <summary>
        /// The beatmap to be played is being selected from the pick pool.
        /// </summary>
        WaitForSelection,

        /// <summary>
        /// The next round is starting.
        /// </summary>
        WaitForStart,

        InGameplay
    }
}
