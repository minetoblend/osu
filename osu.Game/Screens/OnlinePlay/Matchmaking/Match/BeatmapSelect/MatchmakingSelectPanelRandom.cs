// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Database;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelRandom : MatchmakingSelectPanel
    {
        public MatchmakingSelectPanelRandom(MultiplayerPlaylistItem item)
            : base(item) { }

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesetStore { get; set; } = null!;

        private SpriteIcon dice = null!;
        private OsuSpriteText label = null!;

        private Sample? resultSample;
        private Sample? swooshSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, OverlayColourProvider colourProvider)
        {
            resultSample = audio.Samples.Get(@"Multiplayer/Matchmaking/Selection/roulette-result");
            swooshSample = audio.Samples.Get(@"SongSelect/options-pop-out");

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Dark5,
                },
                new TrianglesV2
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.1f,
                },
                label = new OsuSpriteText
                {
                    Y = 20,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Random"
                },
                dice = new SpriteIcon
                {
                    Y = -10,
                    Size = new Vector2(28),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.DiceThree,
                }
            });

            dice.Spin(10_000, RotationDirection.Clockwise);

            AddInternal(dice.CreateProxy());
        }

        public override void PresentAsChosenBeatmap(MultiplayerPlaylistItem item)
        {
            const double duration = 1000;

            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, duration, Easing.OutExpo);

            dice.MoveToY(-200, duration * 0.5, Easing.OutCubic)
                .Then()
                .MoveToY(-DrawHeight / 2, duration * 0.5, Easing.InCubic)
                .Then()
                .FadeOut()
                .Expire();

            dice.RotateTo(dice.Rotation - 360 * 5, duration * 1.3f, Easing.Out);

            label.FadeOut(200).Expire();

            swooshSample?.Play();

            var contentAsync = loadContentAsync(item);

            Scheduler.AddDelayed(() =>
            {
                resultSample?.Play();

                ShowChosenBorder();

                // ScaleContainer.ScaleTo(0.95f, 120, Easing.Out)
                //               .Then()
                //               .ScaleTo(1f, 600, Easing.OutElasticHalf);

                Task.Run(() => showContentWhenReady(contentAsync));
            }, duration);
        }

        private async Task<BeatmapCardMatchmakingBeatmapContent?> loadContentAsync(MultiplayerPlaylistItem item)
        {
            Ruleset? ruleset = rulesetStore.GetRuleset(item.RulesetID)?.CreateInstance();

            if (ruleset == null)
                return null;

            Mod[] mods = item.RequiredMods.Select(m => m.ToMod(ruleset)).ToArray();

            APIBeatmap? beatmap = await beatmapLookupCache.GetBeatmapAsync(item.BeatmapID).ConfigureAwait(false);

            beatmap ??= new APIBeatmap
            {
                BeatmapSet = new APIBeatmapSet
                {
                    Title = "unknown beatmap",
                    TitleUnicode = "unknown beatmap",
                    Artist = "unknown artist",
                    ArtistUnicode = "unknown artist",
                }
            };

            beatmap.StarRating = Item.StarRating;

            return new BeatmapCardMatchmakingBeatmapContent(beatmap, mods);
        }

        private async Task showContentWhenReady(Task<BeatmapCardMatchmakingBeatmapContent?> contentAsync)
        {
            var content = await contentAsync.ConfigureAwait(false);

            if (content == null)
                return;

            Scheduler.Add(() =>
            {
                var flashLayer = new Box { RelativeSizeAxes = Axes.Both };

                CircularContainer maskingContainer;
                //
                var contentWrapper = new BufferedContainer(pixelSnapping: false)
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = maskingContainer = new CircularContainer
                    {
                        Child = new BufferedContainer(pixelSnapping: false)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                content,
                                flashLayer
                            }
                        },
                        Masking = true,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.TopCentre,
                    },
                };

                AddRange(new Drawable[]
                {
                    content,
                    // flashLayer,
                });

                maskingContainer.ResizeTo(new Vector2(DrawWidth * 1.15f), 2000, Easing.OutExpo);

                flashLayer.FadeOutFromOne(1000).Expire();
            });
        }
    }
}
