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
        public void TestSingleSelectionMode()
        {
            AddStep("add cards", () =>
            {
                cardHand.Clear();
                for (int i = 0; i < 5; i++)
                    cardHand.AddCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));
            });
            AddStep("single selection mode", () => cardHand.SelectionMode = CardSelectionMode.Single);

            AddStep("click first card", () => cardHand.Cards.First().TriggerClick());
            AddAssert("first card selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.First().Item]));

            AddStep("click second card", () => cardHand.Cards.ElementAt(1).TriggerClick());
            AddAssert("second card selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.ElementAt(1).Item]));

            AddStep("click second card again", () => cardHand.Cards.ElementAt(1).TriggerClick());
            AddAssert("second card selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.ElementAt(1).Item]));
        }

        [Test]
        public void TestMultiSelectionMode()
        {
            AddStep("add cards", () =>
            {
                cardHand.Clear();
                for (int i = 0; i < 5; i++)
                    cardHand.AddCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));
            });
            AddStep("single selection mode", () => cardHand.SelectionMode = CardSelectionMode.Multiple);

            AddStep("click first card", () => cardHand.Cards.First().TriggerClick());
            AddAssert("first card selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.First().Item]));

            AddStep("click second card", () => cardHand.Cards.ElementAt(1).TriggerClick());
            AddAssert("both cards selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.ElementAt(0).Item, cardHand.Cards.ElementAt(1).Item]));

            AddStep("click second card again", () => cardHand.Cards.ElementAt(1).TriggerClick());
            AddAssert("first card selected", () => cardHand.Selection.SequenceEqual([cardHand.Cards.ElementAt(0).Item]));
        }

        [Test]
        public void TestCardCount()
        {
            for (int i = 1; i <= 8; i++)
            {
                int numCards = i;

                AddStep($"{i} {"cards".Pluralize(i == 1)}", () =>
                {
                    cardHand.Clear();

                    for (int j = 0; j < numCards; j++)
                        cardHand.AddCard(new RankedPlayCardWithPlaylistItem(new RankedPlayCardItem()));
                });
            }
        }
    }
}
