// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using Nito.Disposables.Internals;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Overlays;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;
using Logger = osu.Framework.Logging.Logger;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayCard2 : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        private PlayerCardHand cardHand = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = cardHand = new PlayerCardHand
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.Both,
                Height = 0.5f,
                SelectionMode = CardSelectionMode.Single,
            };
        }

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        [Test]
        public void TestCard()
        {
            AddStep("add card", () =>
            {
                var beatmapsTask = beatmapLookupCache.GetBeatmapsAsync([3631491, 3666654, 3716286, 3658033, 2069833]);
                beatmapsTask.WaitSafely();
                var beatmaps = beatmapsTask.GetResultSafely().WhereNotNull()
                                           .ToArray();

                Logger.Log($"beatmap count: {beatmaps.Length}");

                foreach (var beatmap in beatmaps)
                {
                    var card = new RankedPlayCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));

                    cardHand.AddCard(card);

                    Schedule(() => card.SetContent(new RankedPlayCardFrontSide(beatmap)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }, false));
                }
            });
        }

        private APIBeatmap createBeatmap(int id, double starRating)
        {
            var beatmap = CreateAPIBeatmap();

            beatmap.DifficultyName = "Hard";
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

            beatmap.StarRating = starRating;

            beatmap.BeatmapSet!.Covers = beatmap.BeatmapSet.Covers with
            {
                Cover = $"https://assets.ppy.sh/beatmaps/{id}/covers/cover.jpg"
            };

            return beatmap;
        }
    }
}
