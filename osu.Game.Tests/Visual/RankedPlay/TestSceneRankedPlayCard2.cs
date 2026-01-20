// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using Nito.Disposables.Internals;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Utils;
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
                    var tagsShuffled = all_tags.ToArray();
                    Random.Shared.Shuffle(tagsShuffled);

                    beatmap.BeatmapSet!.RelatedTags = all_tags;
                    beatmap.TopTags = tagsShuffled.Take(RNG.Next(3, 5))
                                                  .Select(tag => new APIBeatmapTag { TagId = tag.Id, VoteCount = RNG.Next(5, 20) })
                                                  .OrderByDescending(tag => tag.VoteCount)
                                                  .ToArray();

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

        private readonly APITag[] all_tags =
        [
            new APITag
            {
                Id = 1,
                Name = "style/messy",
                RulesetId = 0,
                Description = "Visually chaotic and intentionally disorganised patterns, often involving many overlaps and unequal visual spacing between objects."
            },
            new APITag
            {
                Id = 2,
                Name = "expression/simple",
                RulesetId = null,
                Description = "Accessible and straightforward map design."
            },
            new APITag
            {
                Id = 3,
                Name = "expression/chaotic",
                RulesetId = 0,
                Description = "Unpredictable map design, often testing unusual skillsets."
            },
            new APITag
            {
                Id = 4,
                Name = "style/geometric",
                RulesetId = 0,
                Description = "Incorporates geometric shapes within the map's visual design."
            },
            new APITag
            {
                Id = 5,
                Name = "style/freeform",
                RulesetId = 0,
                Description = "An unrestrained and loose approach towards visual structure."
            },
            new APITag
            {
                Id = 6,
                Name = "style/grid snap",
                RulesetId = 0,
                Description = "Objects are placed along a square grid, typically using osu!'s in-built grid snap feature."
            },
            new APITag
            {
                Id = 7,
                Name = "style/hexgrid",
                RulesetId = 0,
                Description = "Objects are placed along a hexagonal grid."
            },
            new APITag
            {
                Id = 8,
                Name = "style/symmetrical",
                RulesetId = 0,
                Description = "Employs symmetry within the map design, often mirroring elements along the horizontal centreline."
            },
            new APITag
            {
                Id = 9,
                Name = "gimmick/playfield constraint",
                RulesetId = 0,
                Description = "Restricts object placement to a certain part of the playfield."
            },
            new APITag
            {
                Id = 10,
                Name = "expression/old-style revival",
                RulesetId = 0,
                Description = "Emulates a style closely associated with early mapping, typically to pay homage or invoke nostalgia."
            },
            new APITag
            {
                Id = 11,
                Name = "expression/repetition",
                RulesetId = null,
                Description = "Features the use of recognizable identical patterns or gameplay elements."
            },
            new APITag
            {
                Id = 12,
                Name = "expression/progression",
                RulesetId = null,
                Description = "Contains a gradual advancement in difficulty or concept across the map."
            },
            new APITag
            {
                Id = 13,
                Name = "expression/high contrast",
                RulesetId = null,
                Description = "Uses flashy ideas to follow changes in the music, creating stark differences between different sections of the song."
            },
            new APITag
            {
                Id = 14,
                Name = "expression/improvisation",
                RulesetId = 0,
                Description = "Uses patterns that do not correspond to sounds in the music, often using hitsounds to add sounds in."
            },
            new APITag
            {
                Id = 15,
                Name = "skillset/tech",
                RulesetId = 0,
                Description = "Tests uncommon skills."
            },
            new APITag
            {
                Id = 16,
                Name = "tech/slider tech",
                RulesetId = 0,
                Description = "Tests uncommon skills involving sliders, such as heavy use of kicksliders or aim on complex slidershapes."
            },
            new APITag
            {
                Id = 17,
                Name = "tech/finger control",
                RulesetId = 0,
                Description = "Uses complex applications of rhythms in order to test the player's tapping ability."
            },
            new APITag
            {
                Id = 18,
                Name = "sliders/complex sv",
                RulesetId = 0,
                Description = "Changes slider velocity by large amounts in order to test the player's reading and aim ability."
            },
            new APITag
            {
                Id = 19,
                Name = "skillset/jumps",
                RulesetId = 0,
                Description = "Focuses heavily on jumps, i.e. circles spaced far apart that require the player to move towards, slow down to hit, then speed up to move towards the next object."
            },
            new APITag
            {
                Id = 20,
                Name = "jumps/sharp",
                RulesetId = 0,
                Description = "Patterns with heavy use of sharp angle movement."
            },
            new APITag
            {
                Id = 21,
                Name = "jumps/wide",
                RulesetId = 0,
                Description = "Patterns with heavy use of wide angle movement."
            },
            new APITag
            {
                Id = 22,
                Name = "jumps/linear",
                RulesetId = 0,
                Description = "Patterns that require the player to move continuously in a straight or nearly straight line."
            },
            new APITag
            {
                Id = 23,
                Name = "tech/aim control",
                RulesetId = 0,
                Description = "Patterns with velocity or direction changes which strongly go against a player's natural movement pattern."
            },
            new APITag
            {
                Id = 24,
                Name = "streams/flow aim",
                RulesetId = 0,
                Description = "Patterns with fully continuous cursor movement, usually due to a combination of wide angles and little time between objects."
            },
            new APITag
            {
                Id = 25,
                Name = "streams/bursts",
                RulesetId = null,
                Description = "Patterns requiring consecutively tapping 5-9 note groups."
            },
            new APITag
            {
                Id = 26,
                Name = "skillset/streams",
                RulesetId = null,
                Description = "Patterns requiring continuous note hits, typically more than 9 notes."
            },
            new APITag
            {
                Id = 27,
                Name = "streams/spaced streams",
                RulesetId = 0,
                Description = "Streams with large spacing, typically ones where the notes don't overlap."
            },
            new APITag
            {
                Id = 28,
                Name = "streams/stamina",
                RulesetId = 0,
                Description = "Tests a player's ability to tap dense rhythms over long periods of time."
            },
            new APITag
            {
                Id = 29,
                Name = "streams/cutstreams",
                RulesetId = 0,
                Description = "Streams in which the spacing of certain notes is much larger than the rest of the stream."
            },
            new APITag
            {
                Id = 30,
                Name = "skillset/reading",
                RulesetId = null,
                Description = "Tests a player's reading skill, i.e. patterns that obfuscate note order and/or timing."
            },
            new APITag
            {
                Id = 31,
                Name = "reading/visually dense",
                RulesetId = 0,
                Description = "Contains patterns where the amount of visible notes makes determining note order and/or timing difficult."
            },
            new APITag
            {
                Id = 32,
                Name = "reading/overlaps",
                RulesetId = 0,
                Description = "Contains patterns where overlapped objects make determining note order and/or timing difficult."
            },
            new APITag
            {
                Id = 33,
                Name = "skillset/precision",
                RulesetId = 0,
                Description = "Colloquial term for maps which require fine, precise movement to aim correctly. Typically refers to maps with circle sizes above and including 6."
            },
            new APITag
            {
                Id = 34,
                Name = "skillset/alt",
                RulesetId = 0,
                Description = "Colloquial term for maps which use rhythms that encourage the player to alternate notes. Typically distinct from burst or stream maps."
            },
            new APITag
            {
                Id = 35,
                Name = "meta/collab",
                RulesetId = null,
                Description = "A map with two or more associated mappers."
            },
            new APITag
            {
                Id = 38,
                Name = "meta/multi-song",
                RulesetId = null,
                Description = "A map containing multiple songs."
            },
            new APITag
            {
                Id = 39,
                Name = "meta/variable timing",
                RulesetId = null,
                Description = "Contains multiple timing points, usually required for songs not recorded to a metronome."
            },
            new APITag
            {
                Id = 40,
                Name = "meta/time signatures",
                RulesetId = null,
                Description = "A song featuring many changes in time signature or uses an uncommon time signature."
            },
            new APITag
            {
                Id = 41,
                Name = "style/clean",
                RulesetId = 0,
                Description = "Visually uncluttered and organised patterns, often involving few overlaps and equal visual spacing between objects."
            },
            new APITag
            {
                Id = 42,
                Name = "sliders/complex slidershapes",
                RulesetId = 0,
                Description = "Uses a large variety of slider designs."
            },
            new APITag
            {
                Id = 43,
                Name = "gimmick/aspire",
                RulesetId = 0,
                Description = "Uses glitches to provide gameplay or visual effects that are otherwise impossible to achieve, originating from the annual Aspire mapping contest."
            },
            new APITag
            {
                Id = 44,
                Name = "style/distance snap",
                RulesetId = 0,
                Description = "Uses osu's in-built distance snap feature for most/all of the map."
            },
            new APITag
            {
                Id = 45,
                Name = "context/mapping contest",
                RulesetId = null,
                Description = "An entry for a mapping contest."
            },
            new APITag
            {
                Id = 46,
                Name = "context/tournament custom",
                RulesetId = null,
                Description = "A map created for a playing tournament."
            },
            new APITag
            {
                Id = 48,
                Name = "tech/complex snap",
                RulesetId = 1,
                Description = "Maps that feature prominent usage of mixed or unusual snap divisors."
            },
            new APITag
            {
                Id = 49,
                Name = "expression/iNiS-style",
                RulesetId = 0,
                Description = "A style originating from the original DS games, recognizable by its emphasis on vocal rhythm, constant slider velocity, and easily understandable grid-snapped patterns."
            },
            new APITag
            {
                Id = 50,
                Name = "gimmick/2B",
                RulesetId = null,
                Description = "Includes gameplay elements with two or more objects placed simultaneously. The term originates from a Chinese transliteration of 'idiot'."
            },
            new APITag
            {
                Id = 51,
                Name = "expression/playfield usage",
                RulesetId = 0,
                Description = "Includes deliberate use of the playfield within the map design."
            },
            new APITag
            {
                Id = 52,
                Name = "gimmick/tag",
                RulesetId = null,
                Description = "Includes gameplay designed for the multiplayer tag mode, where multiple players complete a map co-operatively."
            },
            new APITag
            {
                Id = 53,
                Name = "expression/difficulty spike",
                RulesetId = null,
                Description = "Contains a sudden and significant increase in challenge within the map's gameplay."
            },
            new APITag
            {
                Id = 54,
                Name = "style/avant-garde",
                RulesetId = null,
                Description = "Employs boundary-pushing and experimental philosophies to map design, often foregoing gameplay and aesthetic conventions to extreme measures."
            },
            new APITag
            {
                Id = 55,
                Name = "additions/keysounds",
                RulesetId = null,
                Description = "Contains hitsounds that use various pitched samples to create a melody, typically following one within the song."
            },
            new APITag
            {
                Id = 56,
                Name = "gimmick/storyboard",
                RulesetId = null,
                Description = "Includes a storyboard that changes how the map is played, usually by changing a map's visuals by using storyboard elements in place of showing the map's hitobjects."
            },
            new APITag
            {
                Id = 57,
                Name = "sliders/low sv",
                RulesetId = null,
                Description = "Features prominent low slider velocity usage as a key part of map design."
            },
            new APITag
            {
                Id = 58,
                Name = "sliders/high sv",
                RulesetId = null,
                Description = "Features prominent high slider velocity usage as a key part of map design."
            },
            new APITag
            {
                Id = 59,
                Name = "meta/mega collab",
                RulesetId = null,
                Description = "A map with 8 or more associated mappers."
            },
            new APITag
            {
                Id = 60,
                Name = "gimmick/slider only",
                RulesetId = 0,
                Description = "Restricts object choice to sliders only."
            },
            new APITag
            {
                Id = 61,
                Name = "gimmick/circle only",
                RulesetId = 0,
                Description = "Restricts object choice to circles only."
            },
            new APITag
            {
                Id = 63,
                Name = "additions/custom skin",
                RulesetId = null,
                Description = "Utilizes custom skin elements and graphics."
            },
            new APITag
            {
                Id = 64,
                Name = "gimmick/video",
                RulesetId = null,
                Description = "Employs patterning that closely references the included background video."
            },
            new APITag
            {
                Id = 66,
                Name = "reading/perfect stacks",
                RulesetId = 0,
                Description = "Features perfectly overlapped stacked notes using low stack leniency."
            },
            new APITag
            {
                Id = 67,
                Name = "gimmick/ninja spinners",
                RulesetId = 0,
                Description = "Features spinners that are very short in duration."
            },
            new APITag
            {
                Id = 70,
                Name = "meta/accelerating bpm",
                RulesetId = null,
                Description = "A song featuring progressively increasing tempo."
            },
            new APITag
            {
                Id = 71,
                Name = "context/custom song",
                RulesetId = null,
                Description = "Maps a song made specifically for the map. This includes songs created for a mapping contest which the map participated in."
            },
            new APITag
            {
                Id = 72,
                Name = "gimmick/mirrored",
                RulesetId = 1,
                Description = "A map that features patterns that are mirrored on an axes in quick successions."
            },
            new APITag
            {
                Id = 73,
                Name = "style/vocal",
                RulesetId = 1,
                Description = "Patterning that focuses mainly on vocals."
            },
            new APITag
            {
                Id = 74,
                Name = "style/tnt",
                RulesetId = 1,
                Description = "A map that imitates the mapping style in Taiko No Tatsujin."
            },
            new APITag
            {
                Id = 75,
                Name = "streams/speed",
                RulesetId = null,
                Description = "A map that requires constant tapping at high BPMs."
            },
            new APITag
            {
                Id = 76,
                Name = "style/double bpm",
                RulesetId = 1,
                Description = "A map that plays at double the speed than what the BPM indicates."
            },
            new APITag
            {
                Id = 77,
                Name = "style/finisher-heavy",
                RulesetId = 1,
                Description = "Features finishers used in an unconventional manner or in large amounts."
            },
            new APITag
            {
                Id = 78,
                Name = "style/mono-heavy",
                RulesetId = 1,
                Description = "Features monos used in large amounts."
            },
            new APITag
            {
                Id = 79,
                Name = "gimmick/reversed",
                RulesetId = 1,
                Description = "A map using reversed patterns in a regularly consecutive manner."
            },
            new APITag
            {
                Id = 80,
                Name = "gimmick/yellow notes",
                RulesetId = 1,
                Description = "A map featuring frequent use of extremely short sliders to simulate ghost notes."
            },
            new APITag
            {
                Id = 82,
                Name = "gimmick/barlines",
                RulesetId = 1,
                Description = "A map that makes use of barlines to enhance visuals or replace notes."
            },
            new APITag
            {
                Id = 83,
                Name = "gimmick/memory",
                RulesetId = null,
                Description = "A map designed around a memorization concept."
            },
            new APITag
            {
                Id = 84,
                Name = "style/taikosu",
                RulesetId = 1,
                Description = "A map designed with both osu! and osu!taiko in mind."
            },
            new APITag
            {
                Id = 85,
                Name = "meta/swing",
                RulesetId = null,
                Description = "A song with prominent use of swing rhythms."
            },
            new APITag
            {
                Id = 86,
                Name = "expression/improvisation",
                RulesetId = 1,
                Description = "A map that's based on full improvisation that works as additional layer to the song."
            },
            new APITag
            {
                Id = 87,
                Name = "skillset/tech",
                RulesetId = 1,
                Description = "A map that frequently makes use of complex snaps."
            },
            new APITag
            {
                Id = 88,
                Name = "style/convert",
                RulesetId = 1,
                Description = "A map that imitates the converted maps from osu!."
            },
            new APITag
            {
                Id = 89,
                Name = "additions/hitsounds",
                RulesetId = 1,
                Description = "A map that makes use of non-default hitsound samplesets."
            },
            new APITag
            {
                Id = 90,
                Name = "skillset/tech",
                RulesetId = 2,
                Description = "A map with the focus on lots of 1/4 sliders, hypersliders and stacks."
            },
            new APITag
            {
                Id = 91,
                Name = "tech/flow",
                RulesetId = 2,
                Description = "A map focused on natural and intuitive movement patterns."
            },
            new APITag
            {
                Id = 92,
                Name = "tech/antiflow",
                RulesetId = 2,
                Description = "A map focused on strong direction or velocity changes that go against a player's natural movement pattern."
            },
            new APITag
            {
                Id = 93,
                Name = "tech/jump",
                RulesetId = 2,
                Description = "A map that focuses on 1/2 constant dashes or hyperdashes."
            },
            new APITag
            {
                Id = 94,
                Name = "gimmick/no hyperdashes",
                RulesetId = 2,
                Description = "A map that doesn’t use hyperdashes even if allowed for its respective difficulty."
            },
            new APITag
            {
                Id = 95,
                Name = "gimmick/long sliders",
                RulesetId = 2,
                Description = "A map that uses sliders for an extended period of time, having regular gameplay around catching juice drops and droplets instead of fruits."
            },
            new APITag
            {
                Id = 96,
                Name = "gimmick/spinner-heavy",
                RulesetId = 2,
                Description = "A map that features heavy usage of spinners."
            },
            new APITag
            {
                Id = 97,
                Name = "style/convert",
                RulesetId = 2,
                Description = "A map that imitates the converted maps from osu!, where the structure and distances are irregular."
            },
            new APITag
            {
                Id = 98,
                Name = "skillset/precision",
                RulesetId = 2,
                Description = "Colloquial term for maps which require fine, precise movement to catch fruits. Typically refers to maps with circle sizes above and including 6."
            },
            new APITag
            {
                Id = 99,
                Name = "tech/wiggles",
                RulesetId = 2,
                Description = "A map with a focus on quick directional changes."
            },
            new APITag
            {
                Id = 100,
                Name = "tech/hyperwalks",
                RulesetId = 2,
                Description = "A map which makes use of hyperdashes that require the player to walk, as otherwise, you will overshoot them."
            },
            new APITag
            {
                Id = 101,
                Name = "gimmick/dodge the beat",
                RulesetId = 2,
                Description = "A map where the player needs to avoid every object."
            },
            new APITag
            {
                Id = 102,
                Name = "style/jumpstream",
                RulesetId = 3,
                Description = "Stream with a mix of 2-note sized chords"
            },
            new APITag
            {
                Id = 103,
                Name = "style/handstream",
                RulesetId = 3,
                Description = "Stream with a mix of 3-note sized chords"
            },
            new APITag
            {
                Id = 104,
                Name = "style/quadstream",
                RulesetId = 3,
                Description = "Stream with a mix of 4-note sized chords"
            },
            new APITag
            {
                Id = 105,
                Name = "style/chordjack",
                RulesetId = 3,
                Description = "Maps that feature evenly spaced chords, with several consecutive notes stacked on the same columns."
            },
            new APITag
            {
                Id = 106,
                Name = "style/longjack",
                RulesetId = 3,
                Description = "Maps that feature long chains of consecutive notes on the same column."
            },
            new APITag
            {
                Id = 107,
                Name = "skillset/speedjack",
                RulesetId = 3,
                Description = "Maps that feature shorter jack sequences, characterized by the faster timeframe between consecutive stacked notes."
            },
            new APITag
            {
                Id = 108,
                Name = "skillset/wristjack",
                RulesetId = 3,
                Description = "Fast and/or fairly dense chordjack maps, whose optimal playing technique involves the use of the player’s wrist movements."
            },
            new APITag
            {
                Id = 109,
                Name = "style/chordstream",
                RulesetId = 3,
                Description = "Maps that make use of streams with a mix of differently-sized chords"
            },
            new APITag
            {
                Id = 110,
                Name = "gimmick/delay",
                RulesetId = 3,
                Description = "Maps that feature high snap streams based on the song’s delayed sound effect. "
            },
            new APITag
            {
                Id = 111,
                Name = "skillset/tech",
                RulesetId = 3,
                Description = "A map that frequently makes use of complex snaps."
            },
            new APITag
            {
                Id = 112,
                Name = "style/mixed rice",
                RulesetId = 3,
                Description = "Maps that make use of multiple rice patterning styles."
            },
            new APITag
            {
                Id = 113,
                Name = "style/LN coordination",
                RulesetId = 3,
                Description = "Maps which require holding multiple long notes simultaneously while hitting other patterns."
            },
            new APITag
            {
                Id = 114,
                Name = "style/LN release",
                RulesetId = 3,
                Description = "Maps that make use of different long note holds ending."
            },
            new APITag
            {
                Id = 115,
                Name = "gimmick/LN inverse",
                RulesetId = 3,
                Description =
                    "Maps that feature long note holds that emphasize constant holds and releases in quick succession. Most distinct feature of Inverse is the “negative spaces” in the patterning."
            },
            new APITag
            {
                Id = 116,
                Name = "style/LN density",
                RulesetId = 3,
                Description = "Maps that feature dense long note streams without breaks."
            },
            new APITag
            {
                Id = 117,
                Name = "style/LN mixed ",
                RulesetId = 3,
                Description = "Maps that make use of multiple long note patterning styles."
            },
            new APITag
            {
                Id = 118,
                Name = "style/generic hybrid",
                RulesetId = 3,
                Description = "Maps that feature the combination of both straightforward rice and Long Notes patterning."
            },
            new APITag
            {
                Id = 119,
                Name = "tech/technical hybrid",
                RulesetId = 3,
                Description = "Maps that feature the combination of both technical rice and Long Notes patterning."
            },
            new APITag
            {
                Id = 120,
                Name = "style/tiebreaker",
                RulesetId = 3,
                Description = "Maps that contain most of the skill sets from different categories, and are usually longer than 5 minutes."
            },
            new APITag
            {
                Id = 121,
                Name = "style/o2jam",
                RulesetId = 3,
                Description = "Map that mimics traditional mapping techniques usually found in O2jam charts."
            },
            new APITag
            {
                Id = 122,
                Name = "style/N+1",
                RulesetId = 3,
                Description = "A specific type of playstyle where the leftmost column is mapped independently from the rest of the columns, which otherwise form a standard playstyle."
            },
            new APITag
            {
                Id = 123,
                Name = "style/dump",
                RulesetId = 3,
                Description =
                    "Maps that use groups of objects focusing more on the extension and intensity of the sounds, in contrast with using individual notes to follow each's sound timing accurately."
            },
            new APITag
            {
                Id = 124,
                Name = "sliders/complex sv",
                RulesetId = 1,
                Description = "Changes slider velocity by large amounts in order to test the player's reading ability."
            },
            new APITag
            {
                Id = 125,
                Name = "sliders/complex sv",
                RulesetId = 3,
                Description = "Changes slider velocity by large amounts in order to test the player's reading ability."
            },
            new APITag
            {
                Id = 128,
                Name = "streams/stamina",
                RulesetId = 1,
                Description = "Tests a player's ability to tap dense rhythms over long periods of time."
            },
            new APITag
            {
                Id = 129,
                Name = "streams/stamina",
                RulesetId = 3,
                Description = "Tests a player's ability to tap dense rhythms over long periods of time."
            },
            new APITag
            {
                Id = 130,
                Name = "jumps/squares",
                RulesetId = 0,
                Description = "Frequent use of square patterns."
            },
            new APITag
            {
                Id = 131,
                Name = "jumps/triangles",
                RulesetId = 0,
                Description = "Frequent use of triangular patterns."
            },
            new APITag
            {
                Id = 132,
                Name = "jumps/cross-screen",
                RulesetId = 0,
                Description = "Frequent use of jumps with a lot of spacing, usually placed on opposite sides of the playfield."
            },
            new APITag
            {
                Id = 133,
                Name = "jumps/stamina",
                RulesetId = 0,
                Description = "Tests a player's ability to aim spaced jumps over long periods of time."
            },
            new APITag
            {
                Id = 135,
                Name = "tech/complex snap",
                RulesetId = 2,
                Description = "Maps that feature prominent usage of mixed or unusual snap divisors."
            },
            new APITag
            {
                Id = 136,
                Name = "tech/complex snap",
                RulesetId = 3,
                Description = "Maps that feature prominent usage of mixed or unusual snap divisors."
            },
            new APITag
            {
                Id = 137,
                Name = "skillset/gimmick",
                RulesetId = null,
                Description = "Distinct or obscure gameplay elements that cannot be categorised with common skillsets."
            },
            new APITag
            {
                Id = 138,
                Name = "jumps/stars",
                RulesetId = 0,
                Description = "Frequent use of star/pentagon patterns."
            },
            new APITag
            {
                Id = 139,
                Name = "jumps/back and forth",
                RulesetId = 0,
                Description = "Frequent use of back and forth patterns."
            },
            new APITag
            {
                Id = 140,
                Name = "jumps/freeform",
                RulesetId = 0,
                Description = "Frequent use of jumps without distinct patterning."
            },
            new APITag
            {
                Id = 141,
                Name = "streams/doubles",
                RulesetId = null,
                Description = "Patterns requiring consecutively tapping 2 note groups."
            },
            new APITag
            {
                Id = 142,
                Name = "streams/quads",
                RulesetId = null,
                Description = "Patterns requiring consecutively tapping 4 note groups."
            },
            new APITag
            {
                Id = 143,
                Name = "expression/inspo",
                RulesetId = null,
                Description = "Maps that draw inspiration directly from other maps/mappers."
            },
            new APITag
            {
                Id = 144,
                Name = "expression/conceptual",
                RulesetId = null,
                Description = "Distinct map design choices that do not follow ordinary mapping conventions, typically to creatively express part of a song."
            },
            new APITag
            {
                Id = 145,
                Name = "additions/combo colours",
                RulesetId = 0,
                Description = "Maps that adjust combo colours in conjunction with variations in the song, also referred to as colourhax."
            }
        ];
    }
}
