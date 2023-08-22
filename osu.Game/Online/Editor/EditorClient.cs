// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace osu.Game.Online.Editor
{
    public abstract partial class EditorClient : Component, IEditorClient
    {
        /// <summary>
        /// Whether the <see cref="EditorClient"/> is currently connected.
        /// This is NOT thread safe and usage should be scheduled.
        /// </summary>
        public abstract IBindable<bool> IsConnected { get; }

        /// <summary>
        /// Creates an <see cref="EditorRoom"/> for a given beatmap.
        /// </summary>
        /// <param name="beatmap">The serialized beatmap containing the difficulty and all associated files.</param>
        /// <returns>The joined <see cref="EditorRoom"/>.</returns>
        protected abstract Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap);

        /// <summary>
        /// Joins the <see cref="EditorRoom"/> with a given ID.
        /// </summary>
        /// <param name="roomId">The room ID.</param>
        /// <returns>The joined <see cref="EditorRoom"/>.</returns>
        protected abstract Task<EditorRoomJoinedResult> JoinRoom(long roomId);

        /// <summary>
        /// Submits a list of serialized commands to the server.
        /// </summary>
        /// <param name="commands">The commands to submit.</param>
        /// <returns></returns>
        protected abstract Task SubmitCommands(SerializedEditorCommands commands);

        public Task UserJoined(EditorRoomUser user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task UserLeft(EditorRoomUser user)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task RoomClosed()
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task UserStateChanged(int userId, EditorUserState state)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task CommandsSubmitted(EditorCommandEvent commands)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
