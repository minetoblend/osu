// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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
                };

                IsConnected.BindTo(connector.IsConnected);
            }
        }
    }
}
