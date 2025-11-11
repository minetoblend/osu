// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Game.Online.Rooms;
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
        }

        protected override void PopulateCard(BeatmapCardMatchmaking card) => card.DisplayItem(Item);

        public override void PresentAsChosenBeatmap()
        {
            ShowChosenBorder();

            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, 1000, Easing.OutExpo);

            resultSample?.Play();
        }
    }
}
