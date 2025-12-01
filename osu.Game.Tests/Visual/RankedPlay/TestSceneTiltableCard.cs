// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Tests.Visual.Matchmaking;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public class TestSceneTiltableCard : MatchmakingTestScene
    {
        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            var beatmap = CreateAPIBeatmap();

            beatmap.BeatmapSet!.Ratings = Enumerable.Range(0, 11).ToArray();
            beatmap.BeatmapSet!.RelatedTags =
            [
                new APITag
                {
                    Id = 2,
                    Name = "song representation/simple",
                    Description = "Accessible and straightforward map design."
                },
                new APITag
                {
                    Id = 4,
                    Name = "style/clean",
                    Description = "Visually uncluttered and organised patterns, often involving few overlaps and equal visual spacing between objects."
                },
                new APITag
                {
                    Id = 23,
                    Name = "aim/aim control",
                    Description = "Patterns with velocity or direction changes which strongly go against a player's natural movement pattern."
                }
            ];

            beatmap.TopTags =
            [
                new APIBeatmapTag { TagId = 4, VoteCount = 1 },
                new APIBeatmapTag { TagId = 2, VoteCount = 1 },
                new APIBeatmapTag { TagId = 23, VoteCount = 5 },
            ];

            beatmap.FailTimes = new APIFailTimes
            {
                Fails = Enumerable.Range(1, 100).Select(i => i % 12 - 6).ToArray(),
                Retries = Enumerable.Range(-2, 100).Select(i => i % 12 - 6).ToArray(),
            };

            beatmap.StarRating = 7.49;

            Child = new TiltableSprite(beatmap)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }
    }
}
