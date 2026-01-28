// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Graphics.Cursor;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;
using osu.Game.Tests.Resources;
using osuTK;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneRankedPlayCard : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Purple);

        [Test]
        public void TestCards()
        {
            AddStep("add cards", () =>
            {
                FillFlowContainer flow;

                Child = new OsuContextMenuContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new CardDetailsOverlayContainer
                    {
                        Child = flow = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 800f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new Vector2(10),
                        }
                    }
                };

                for (int i = 0; i < 10; i++)
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
                        Fails = Enumerable.Range(1, 100).Select(x => x % 12 - 6).ToArray(),
                        Retries = Enumerable.Range(-2, 100).Select(x => x % 12 - 6).ToArray(),
                    };

                    beatmap.StarRating = i + 1;

                    flow.Add(new RankedPlayCardContent(beatmap)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(1.2f),
                    });
                }
            });
        }

        [Test]
        public void TestCardHand()
        {
            AddStep("add cards", () =>
            {
                PlayerCardHand cardHand;

                Child = new OsuContextMenuContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new CardDetailsOverlayContainer
                    {
                        Child = cardHand = new PlayerCardHand
                        {
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(0.5f),
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                        },
                    }
                };

                foreach (var beatmap in getBeatmaps())
                {
                    var card = new RankedPlayCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));

                    cardHand.AddCard(card);

                    Schedule(() => card.SetContent(new RankedPlayCardContent(beatmap)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }, false));
                }
            });
        }

        [Resolved]
        private RulesetStore rulesetStore { get; set; } = null!;

        [Test]
        public void TestRulesets()
        {
            var rulesets = rulesetStore.AvailableRulesets.Where(it => it.OnlineID >= 0);

            foreach (var ruleset in rulesets)
            {
                AddStep(ruleset.ShortName, () =>
                {
                    FillFlowContainer flow;

                    Child = new OsuContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = flow = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 800f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Spacing = new Vector2(10),
                        }
                    };

                    for (int i = 0; i < 10; i++)
                    {
                        var beatmap = CreateAPIBeatmap(ruleset);

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
                            Fails = Enumerable.Range(1, 100).Select(x => x % 12 - 6).ToArray(),
                            Retries = Enumerable.Range(-2, 100).Select(x => x % 12 - 6).ToArray(),
                        };

                        beatmap.StarRating = i + 1;

                        flow.Add(new RankedPlayCardContent(beatmap)
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Scale = new Vector2(1.2f),
                        });
                    }
                });
            }
        }

        private APIBeatmap[] getBeatmaps()
        {
            using var resourceStream = TestResources.OpenResource("Requests/api-beatmaps-rankedplay.json");
            using var reader = new StreamReader(resourceStream);

            return JsonConvert.DeserializeObject<APIBeatmap[]>(reader.ReadToEnd())!;
        }
    }
}
