// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Tests.Resources;
using osu.Game.Tests.Visual.Multiplayer;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public abstract partial class RankedPlayTestScene : MultiplayerTestScene
    {
        /// <summary>
        /// Returns 5 sample <see cref="APIBeatmap"/>s.
        /// </summary>
        protected static APIBeatmap[] GetSampleBeatmaps()
        {
            using var resourceStream = TestResources.OpenResource("Requests/api-beatmaps-rankedplay.json");
            using var reader = new StreamReader(resourceStream);

            return JsonConvert.DeserializeObject<APIBeatmap[]>(reader.ReadToEnd())!;
        }

        /// <summary>
        /// A request handler that will resolve api requests to any beatmaps provided by <see cref="GetSampleBeatmaps"/>.
        /// </summary>
        public class BeatmapRequestHandler
        {
            public readonly APIBeatmap[] Beatmaps = GetSampleBeatmaps();

            public bool HandleRequest(APIRequest request)
            {
                switch (request)
                {
                    case GetBeatmapRequest beatmapRequest:
                        var beatmap = Beatmaps.FirstOrDefault(it => it.OnlineID == beatmapRequest.OnlineID);

                        if (beatmap != null)
                        {
                            beatmapRequest.TriggerSuccess(beatmap);
                            return true;
                        }

                        break;

                    case GetBeatmapsRequest beatmapsRequest:
                        beatmapsRequest.TriggerSuccess(new GetBeatmapsResponse
                        {
                            Beatmaps = beatmapsRequest
                                       .BeatmapIds
                                       .Select(id => Beatmaps.FirstOrDefault(it => it.OnlineID == id))
                                       .ToList()
                        });

                        return true;
                }

                return false;
            }
        }
    }
}
