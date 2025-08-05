// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online
{
    public class DevelopmentEndpointConfiguration : EndpointConfiguration
    {
        public DevelopmentEndpointConfiguration()
        {
            WebsiteUrl = APIUrl = @"http://localhost:8080";
            APIClientSecret = @"QqaV4SWds8tnLEpEbhno3vr5aAacAzAvQpZvzofk";
            APIClientID = "1";
            SpectatorUrl = @"http://127.0.0.1:12345/spectator";
            MultiplayerUrl = @"http://127.0.0.1:12345/multiplayer";
            MetadataUrl = @"http://127.0.0.1:12345/metadata";
            BeatmapSubmissionServiceUrl = $@"{APIUrl}/beatmap-submission";
        }
    }
}
