// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Online.API;

namespace osu.Game.Online.Editor
{
    public partial class OnlineEditorClient : EditorClient
    {
        private readonly string endpoint;

        private IHubClientConnector? connector;

        private HubConnection? connection => connector?.CurrentConnection;

        public override IBindable<bool> IsConnected { get; } = new BindableBool();

        public OnlineEditorClient(EndpointConfiguration endpoints)
        {
            endpoint = endpoints.EditorEndpointUrl;
        }

        [BackgroundDependencyLoader]
        private void load(IAPIProvider api)
        {
            connector = api.GetHubConnector(nameof(OnlineEditorClient), endpoint);

            if (connector != null)
            {
                connector.ConfigureConnection = connection =>
                {
                    connection.On<EditorRoomUser>(nameof(IEditorClient.UserJoined), ((IEditorClient)this).UserJoined);
                    connection.On<EditorRoomUser>(nameof(IEditorClient.UserLeft), ((IEditorClient)this).UserLeft);
                    connection.On(nameof(IEditorClient.RoomClosed), ((IEditorClient)this).RoomClosed);
                    connection.On<int, EditorUserState>(nameof(IEditorClient.UserStateChanged), ((IEditorClient)this).UserStateChanged);
                    connection.On<EditorCommandEvent>(nameof(IEditorClient.CommandsSubmitted), ((IEditorClient)this).CommandsSubmitted);
                    connection.On<int, long>(nameof(IEditorClient.Invited), ((IEditorClient)this).Invited);
                };

                IsConnected.BindTo(connector.IsConnected);
            }
        }

        protected override Task<EditorRoom> CreateAndJoinRoom(SerializedEditorBeatmap beatmap)
        {
            if (!IsConnected.Value)
                throw new OperationCanceledException();

            Debug.Assert(connection != null);

            return connection.InvokeAsync<EditorRoom>(nameof(IEditorServer.CreateAndJoinRoom), beatmap);
        }

        public override Task Invite(int userId)
        {
            if (!IsConnected.Value)
                throw new OperationCanceledException();

            Debug.Assert(connection != null);

            return connection.InvokeAsync(nameof(IEditorServer.Invite), userId);
        }

        protected override Task<EditorRoomJoinedResult> JoinRoomInteral(long roomId)
        {
            if (!IsConnected.Value)
                throw new OperationCanceledException();

            Debug.Assert(connection != null);

            return connection.InvokeAsync<EditorRoomJoinedResult>(nameof(IEditorServer.JoinRoom), roomId);
        }
    }
}
