// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Online.API.Requests
{
    public class CopilotRequest : APIRequest<CopilotResult>
    {
        protected override string Target => "copilot";
        protected override string Uri => $"http://localhost:8000/{Target}";

        [JsonProperty("beatmap")]
        public required string BeatmapString { get; init; }

        [JsonProperty("start_time")]
        public required int StartTime { get; init; }

        [JsonProperty("end_time")]
        public required int EndTime { get; init; }

        [JsonProperty("audio_path")]
        public required string AudioPath { get; init; }

        protected override WebRequest CreateWebRequest()
        {
            var req = base.CreateWebRequest();

            req.Method = HttpMethod.Post;
            req.ContentType = @"application/json";
            req.AddRaw(JsonConvert.SerializeObject(this));

            req.Timeout = 30_000;

            return req;
        }
    }
}
