// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Pick;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelBeatmap : MatchmakingSelectPanel
    {
        public new MatchmakingPlaylistItemBeatmap Item => (MatchmakingPlaylistItemBeatmap)base.Item;

        public MatchmakingSelectPanelBeatmap(MatchmakingPlaylistItemBeatmap item)
            : base(item) { }

        private Sample? resultSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            resultSample = audio.Samples.Get(@"Multiplayer/Matchmaking/Selection/roulette-result");

            Add(content = new BeatmapCardMatchmakingBeatmapContent(Item.Beatmap, Item.Mods));
        }

        public override void PresentAsChosenBeatmap(MatchmakingPlaylistItemBeatmap item)
        {
            ShowChosenBorder();

            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, 1000, Easing.OutExpo);

            resultSample?.Play();
        }

        public override void PresentAsUnanimouslyChosenBeatmap(MatchmakingPlaylistItemBeatmap item)
        {
            var flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
            };

            Add(flash);
            OverlayLayer.Add(new TriangleImplosion
            {
                RelativeSizeAxes = Axes.Both,
                Duration = 1000
            });

            this.ScaleTo(0.9f, 1000, Easing.InCubic)
                .Then()
                .ScaleTo(1f, 500, Easing.OutElasticHalf);

            flash.FadeInFromZero(1000, Easing.In)
                 .Then()
                 .FadeOut(1000, Easing.Out)
                 .Expire();

            Scheduler.AddDelayed(() =>
            {
                ShowChosenBorder();

                this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                    .ScaleTo(1.5f, 600, Easing.OutElasticHalf);

                resultSample?.Play();
            }, 1000);
        }

        private BeatmapCardMatchmakingBeatmapContent? content;

        protected override float AvatarOverlayOffset => content?.AvatarOffset ?? 0;
    }
}
