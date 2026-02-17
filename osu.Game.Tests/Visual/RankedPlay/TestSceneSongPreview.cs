// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Online.API;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneSongPreview : RankedPlayTestScene
    {
        private RankedPlayCard card = null!;

        private readonly BeatmapRequestHandler requestHandler = new BeatmapRequestHandler();

        public override void SetUpSteps()
        {
            base.SetUpSteps();

            var beatmap = requestHandler.Beatmaps[1];

            var item = new RevealedRankedPlayCardWithPlaylistItem(beatmap);

            AddStep("setup request handler", () => ((DummyAPIAccess)API).HandleRequest = requestHandler.HandleRequest);

            AddStep("add card", () => Child = card = new RankedPlayCard(item)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }
    }
}
