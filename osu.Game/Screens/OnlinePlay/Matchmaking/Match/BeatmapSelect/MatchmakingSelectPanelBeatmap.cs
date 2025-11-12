// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Database;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Pick;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelBeatmap : MatchmakingSelectPanel
    {
        public MatchmakingSelectPanelBeatmap(MultiplayerPlaylistItem item)
            : base(item) { }

        private Sample? resultSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            resultSample = audio.Samples.Get(@"Multiplayer/Matchmaking/Selection/roulette-result");

            Task.Run(loadContent);
        }

        public override void PresentAsChosenBeatmap(MultiplayerPlaylistItem item)
        {
            ShowChosenBorder();

            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, 1000, Easing.OutExpo);

            resultSample?.Play();
        }

        public override void PresentAsUnanimouslyChosenBeatmap(MultiplayerPlaylistItem item)
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

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        [Resolved]
        private RulesetStore rulesetStore { get; set; } = null!;

        private BeatmapCardMatchmakingBeatmapContent? content;

        private async Task loadContent()
        {
            Ruleset? ruleset = rulesetStore.GetRuleset(Item.RulesetID)?.CreateInstance();

            if (ruleset == null)
                return;

            Mod[] mods = Item.RequiredMods.Select(m => m.ToMod(ruleset)).ToArray();

            APIBeatmap? beatmap = await beatmapLookupCache.GetBeatmapAsync(Item.BeatmapID).ConfigureAwait(false);

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

            Scheduler.Add(() =>
            {
                Add(content = new BeatmapCardMatchmakingBeatmapContent(beatmap, mods));
            });
        }

        protected override float AvatarOverlayOffset => base.AvatarOverlayOffset + (content?.AvatarOffset ?? 0);
    }
}
