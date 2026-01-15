// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using Humanizer;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Overlays;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestScenePlayerCardHand : OsuTestScene
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
            };
        }

        [Test]
        public void TestCardCount()
        {
            for (int i = 1; i <= 8; i++)
            {
                int numCards = i;

                AddStep($"{i} {"cards".Pluralize(i == 1)}", () =>
                {
                    foreach (var card in cardHand.Cards.ToArray())
                        cardHand.RemoveCard(card.Card.Item);

                    for (int j = 0; j < numCards; j++)
                        cardHand.AddCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));
                });
            }
        }
    }
}
