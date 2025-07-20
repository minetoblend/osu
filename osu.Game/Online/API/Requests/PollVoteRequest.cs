// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Online.API.Requests
{
    public class PollVoteRequest : APIRequest<APIPoll>
    {
        [JsonIgnore]
        private readonly long pollId;

        [JsonProperty("optionId")]
        public readonly long OptionId;

        public PollVoteRequest(long pollId, long optionId)
        {
            this.pollId = pollId;
            OptionId = optionId;
        }

        protected override WebRequest CreateWebRequest()
        {
            var req = base.CreateWebRequest();
            req.Method = HttpMethod.Post;
            req.ContentType = @"application/json";
            req.AddRaw(JsonConvert.SerializeObject(this));
            return req;
        }

        protected override string Target => $"polls/{pollId}/vote";
    }
}
