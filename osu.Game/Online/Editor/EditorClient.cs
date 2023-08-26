// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Screens.Edit;

namespace osu.Game.Online.Editor
{
    public abstract partial class EditorClient : Component, IEditorClient, IEditorServer
    {
        [Resolved]
        private Storage storage { get; set; } = null!;

        /// <summary>
        /// Whether the <see cref="EditorClient"/> is currently connected.
        /// This is NOT thread safe and usage should be scheduled.
        /// </summary>
        public abstract IBindable<bool> IsConnected { get; }

        MultiplayerEditorBeatmapSerializer serializer = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            serializer = new MultiplayerEditorBeatmapSerializer(storage);
        }

        public async Task CreateAndJoinRoom(EditorBeatmap beatmap)
        {
            string encodedBeatmap = serializer.SerializeBeatmap(beatmap.PlayableBeatmap);

            var files = await Task.Factory.StartNew<Dictionary<string, byte[]>>(() => serializer.SerializeBeatmapFiles(
                beatmap.BeatmapInfo.BeatmapSet ?? throw new InvalidOperationException("Beatmap must be part of a beatmap set.")
            )).ConfigureAwait(false);

            var room = await CreateAndJoinRoom(new SerializedEditorBeatmap(
                encodedBeatmap,
                files
            )).ConfigureAwait(false);
        }

        protected abstract Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap);

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
