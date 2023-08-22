// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Online.API;

namespace osu.Game.Online.Editor
{
    public partial class OnlineEditorClient : EditorClient
    {
        private readonly string endpoint;

        private IHubClientConnector? connector;

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
                };

                IsConnected.BindTo(connector.IsConnected);
            }
        }

        protected override Task<EditorRoom> CreateAndJoinRoom(IBeatmapInfo beatmap, IBeatmapSetInfo beatmapSet)
        {
            //TODO
            throw new System.NotImplementedException();
        }

        protected override Task<EditorRoom> JoinRoom(long roomId)
        {
            //TODO
            throw new System.NotImplementedException();
        }
    }
}
