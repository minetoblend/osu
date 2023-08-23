// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;

namespace osu.Game.Online.Editor
{
    public interface IEditorServer
    {
        Task LeaveRoom();

        /// <summary>
        /// Creates an <see cref="EditorRoom"/> for a given beatmap.
        /// </summary>
        /// <param name="beatmap">The serialized beatmap containing the difficulty and all associated files.</param>
        /// <returns>The joined <see cref="EditorRoom"/>.</returns>
        Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap);

        /// <summary>
        /// Joins the <see cref="EditorRoom"/> with a given ID.
        /// </summary>
        /// <param name="roomId">The room ID.</param>
        /// <returns>The joined <see cref="EditorRoom"/>.</returns>
        Task<EditorRoomJoinedResult> JoinRoom(long roomId);

        /// <summary>
        /// Submits a list of serialized commands to the server.
        /// </summary>
        /// <param name="commands">The commands to submit.</param>
        Task SubmitCommands(byte[] commands);

        /// <summary>
        /// Change the local user state in the currently joined room.
        /// </summary>
        /// <param name="state"></param>
        Task ChangeState(EditorUserState state);
    }
}
