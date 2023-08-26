// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Database;
using osu.Game.IO;
using osu.Game.Overlays.Notifications;
using osu.Game.Screens.Edit;

namespace osu.Game.Online.Editor
{
    public abstract partial class EditorClient : Component, IEditorClient, IEditorServer
    {
        public Action<Notification>? PostNotification { protected get; set; }

        public Action<WorkingBeatmap>? PostBeatmapLoad { protected get; set; }

        [Resolved]
        private Storage storage { get; set; } = null!;

        [Resolved]
        private UserLookupCache userLookupCache { get; set; } = null!;

        [Resolved]
        private BeatmapManager beatmapManager { get; set; } = null!;

        [Resolved]
        public Bindable<WorkingBeatmap> Beatmap { get; private set; } = null!;

        private EditorRoom? room;

        /// <summary>
        /// The joined <see cref="EditorRoom"/>.
        /// </summary>
        public virtual EditorRoom? Room // virtual for moq
        {
            get
            {
                Debug.Assert(ThreadSafety.IsUpdateThread);
                return room;
            }
            private set
            {
                Debug.Assert(ThreadSafety.IsUpdateThread);
                room = value;
            }
        }

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

            var joinedRoom = await CreateAndJoinRoom(new SerializedEditorBeatmap(
                encodedBeatmap,
                files
            )).ConfigureAwait(false);

            Scheduler.Add(() =>
            {
                Debug.Assert(Room == null);

                Room = joinedRoom;
            });
        }

        public abstract Task Invite(int userId);

        protected abstract Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap);
        protected abstract Task<EditorRoomJoinedResult> JoinRoomInteral(long roomId);

        async Task<EditorRoomJoinedResult> IEditorServer.JoinRoom(long roomId)
        {
            var result = await JoinRoomInteral(roomId).ConfigureAwait(false);

            var beatmap = new LegacyBeatmapDecoder().Decode(
                new LineBufferedReader(new MemoryStream(Encoding.UTF8.GetBytes(result.Beatmap.Beatmap)))
            );

            Scheduler.Add(async () =>
            {
                var workingBeatmap = await beatmapManager.CreateForCollaboration(beatmap, result.Files).ConfigureAwait(true);
                PostBeatmapLoad?.Invoke(workingBeatmap);
            });

            return result;
        }

        Task IEditorClient.UserJoined(EditorRoomUser user)
        {
            return Task.CompletedTask;
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

        public async Task Invited(int userId, long roomId)
        {
            var user = await userLookupCache.GetUserAsync(userId).ConfigureAwait(false);
            if (user == null) return;

            PostNotification?.Invoke(
                new SimpleNotification
                {
                    Text = $"{user.Username} has invited you to edit a beatmap.",

                    Activated = () =>
                    {
                        ((IEditorServer)this).JoinRoom(roomId);
                        return true;
                    }
                });
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

        private Task runOnUpdateThreadAsync(Action action, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();

            Scheduler.Add(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled(cancellationToken);
                    return;
                }

                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
