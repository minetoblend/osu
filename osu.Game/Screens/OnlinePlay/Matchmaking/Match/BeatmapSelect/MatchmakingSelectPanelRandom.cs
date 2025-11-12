// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Database;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelRandom : MatchmakingSelectPanel
    {
        public new MatchmakingPlaylistItemRandom Item => (MatchmakingPlaylistItemRandom)base.Item;

        public MatchmakingSelectPanelRandom(MatchmakingPlaylistItemRandom item)
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

        public override void PresentAsChosenBeatmap(MatchmakingPlaylistItemBeatmap item)
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

            Scheduler.AddDelayed(() =>
            {
                resultSample?.Play();

                ShowChosenBorder();

                ScaleContainer.ScaleTo(0.95f, 120, Easing.Out)
                              .Then()
                              .ScaleTo(1f, 600, Easing.OutElasticHalf);

                var content = new BeatmapCardMatchmakingBeatmapContent(item.Beatmap, item.Mods);

                var flashLayer = new Box { RelativeSizeAxes = Axes.Both };

                AddRange(new Drawable[]
                {
                    content,
                    flashLayer
                });

                flashLayer.FadeOutFromOne(1000).Expire();
            }, duration);
        }
    }
}
