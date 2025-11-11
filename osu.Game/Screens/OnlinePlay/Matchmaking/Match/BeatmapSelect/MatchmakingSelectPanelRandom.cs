// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelRandom : MatchmakingSelectPanel
    {
        public MatchmakingSelectPanelRandom(MultiplayerPlaylistItem item)
            : base(item) { }

        private SpriteIcon dice = null!;
        private OsuSpriteText label = null!;

        private Sample? resultSample;
        private Sample? swooshSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            resultSample = audio.Samples.Get(@"Multiplayer/Matchmaking/Selection/roulette-result");
            swooshSample = audio.Samples.Get(@"SongSelect/options-pop-out");
        }

        protected override void PopulateCard(BeatmapCardMatchmaking card)
        {
            card.DisplayRandom();

            card.AddRange(new Drawable[]
            {
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
                    Size = new Vector2(32),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.DiceThree,
                }
            });

            Add(dice.CreateProxy());
            //
            // ScaleContainer.AddRange(new Drawable[]
            // {
            //     label = new OsuSpriteText
            //     {
            //         Y = 20,
            //         Anchor = Anchor.Centre,
            //         Origin = Anchor.Centre,
            //         Text = "Random"
            //     },
            //     dice = new SpriteIcon
            //     {
            //         Y = -10,
            //         Size = new Vector2(32),
            //         Anchor = Anchor.Centre,
            //         Origin = Anchor.Centre,
            //         Icon = FontAwesome.Solid.DiceThree,
            //     }
            // });
        }

        public override void PresentAsChosenBeatmap()
        {
            ShowChosenBorder();

            const double duration = 1000;

            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, duration, Easing.OutExpo);

            dice.MoveToY(-200, duration / 2, Easing.OutCubic)
                .Then()
                .MoveToY(0, duration / 2, Easing.InCubic)
                .Then()
                .FadeOut()
                .Expire();

            dice.RotateTo(360 * 5, duration, Easing.Out);

            label.FadeOut(350).Expire();

            swooshSample?.Play();

            Scheduler.AddDelayed(() =>
            {
                Card.DisplayItem(new MultiplayerPlaylistItem
                {
                    RulesetID = 0,
                });

                resultSample?.Play();
            }, duration);
        }
    }
}
