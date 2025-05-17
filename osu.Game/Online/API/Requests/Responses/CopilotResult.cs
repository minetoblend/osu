// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Newtonsoft.Json;

namespace osu.Game.Online.API.Requests.Responses
{
    public class CopilotResult
    {
        [JsonProperty(@"success")]
        public bool Success { get; set; }

        [JsonProperty(@"error")]
        public string? Error { get; set; }

        [JsonProperty(@"result")]
        public string? Result { get; set; }
    }
}
