// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace osu.Game.Online.Editor
{
    public abstract partial class EditorClient : Component, IEditorClient, IEditorServer
    {
        /// <summary>
        /// Whether the <see cref="EditorClient"/> is currently connected.
        /// This is NOT thread safe and usage should be scheduled.
        /// </summary>
        public abstract IBindable<bool> IsConnected { get; }

        Task IEditorClient.UserJoined(EditorRoomUser user)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorClient.UserLeft(EditorRoomUser user)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorClient.RoomClosed()
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorClient.UserStateChanged(int userId, EditorUserState state)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorClient.CommandsSubmitted(EditorCommandEvent commands)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task LeaveRoom()
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task<EditorRoomJoinedResult> JoinRoom(long roomId)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task SubmitCommands(SerializedEditorCommands commands)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Task ChangeState(EditorUserState state)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
