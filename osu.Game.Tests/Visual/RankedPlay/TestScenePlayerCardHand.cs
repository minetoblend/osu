// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;
using osuTK;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public partial class TestScenePlayerCardHand : OsuTestScene
    {
        private PlayerCardHand hand = null!;

        [SetUpSteps]
        public void SetupSteps()
        {
            AddStep("add drawable", () => Child = hand = new PlayerCardHand
            {
                Size = new Vector2(750, 300),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            });

            AddStep("add initial cards", () =>
            {
                for (int i = 0; i < 4; i++)
                    hand.AddCard(new RankedPlayCardItem());
            });
        }

        [Test]
        public void TestHand()
        {
            AddStep("draw card", () => hand.DrawCard(new RankedPlayCardItem()));
            AddStep("discard selection", () => hand.DiscardSelectedCards());
            AddToggleStep("allow selection", value => hand.AllowSelection.Value = value);
            AddStep("hidden layout", () => hand.State = PlayerCardHand.CardState.Hidden);
            AddStep("hand layout", () => hand.State = PlayerCardHand.CardState.Hand);
            AddStep("lineup layout", () => hand.State = PlayerCardHand.CardState.Lineup);
        }
    }
}
