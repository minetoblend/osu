// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay;

namespace osu.Game.Tests.Visual.RankedPlay
{
    public class TestScenePlayerCardDeck : OsuTestScene
    {
        public TestScenePlayerCardDeck()
        {
            Child = new PlayerCardDeck
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }.With(d =>
            {
                for (int i = 0; i < 5; i++)
                    d.AddCard(new RankedPlayCard());
            });
        }
    }
}
