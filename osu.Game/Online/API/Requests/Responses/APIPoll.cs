// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using Newtonsoft.Json;

namespace osu.Game.Online.API.Requests.Responses
{
    public class APIPoll
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; } = string.Empty;

        [JsonProperty("total_vote_count")]
        public int TotalVoteCount { get; set; }

        [JsonProperty("options")]
        public APIPollOption[] Options { get; set; } = Array.Empty<APIPollOption>();

        [JsonProperty("has_voted")]
        public bool HasVoted { get; set; }

        [JsonProperty("max_options")]
        public int MaxOptions { get; set; } = 1;
    }
}
