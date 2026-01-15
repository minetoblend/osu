// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Overlays;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;
using osuTK;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestSceneCardHandReplay : TestScene
    {
        private PlayerCardHand playerHand = null!;
        private OpponentCardHand opponentHand = null!;

        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Pink);

        [BackgroundDependencyLoader]
        private void load()
        {
            var cards = Enumerable.Range(0, 5)
                                  .Select(_ => new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()))
                                  .ToArray();

            Children =
            [
                playerHand = new PlayerCardHand
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.5f),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    SelectionMode = CardSelectionMode.Multiple
                },
                opponentHand = new OpponentCardHand
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.5f),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
            ];

            foreach (var card in cards)
            {
                playerHand.AddCard(card);
                opponentHand.AddCard(card);
            }

            playerHand.StateChanged += () => opponentHand.SetState(playerHand.State);
        }
    }
}
