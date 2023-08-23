// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Screens.Edit;

namespace osu.Game.Online.Editor
{
    public abstract partial class EditorClient : Component, IEditorClient, IEditorServer
    {
        /// <summary>
        /// Whether the <see cref="EditorClient"/> is currently connected.
        /// This is NOT thread safe and usage should be scheduled.
        /// </summary>
        public abstract IBindable<bool> IsConnected { get; }

        public async Task CreateAndJoinRoom(EditorBeatmap beatmap)
        {
            await Task.Delay(1000).ConfigureAwait(false);
        }

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

        Task IEditorServer.LeaveRoom()
        {
            // TODO
            throw new NotImplementedException();
        }

        Task<EditorRoom> IEditorServer.CreateAndJoinRoom(SerializedEditorBeatmap beatmap)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task<EditorRoomJoinedResult> IEditorServer.JoinRoom(long roomId)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorServer.SubmitCommands(byte[] commands)
        {
            // TODO
            throw new NotImplementedException();
        }

        Task IEditorServer.ChangeState(EditorUserState state)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
