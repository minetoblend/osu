// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online.Editor
{
    public partial class OnlineEditorClient : EditorClient
    {
        private readonly string endpoint;

        public OnlineEditorClient(EndpointConfiguration endpoints)
        {
            endpoint = endpoints.EditorEndpointUrl;
        }
    }
}
