// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Utils;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardHand
    {
        private static class CardMovement
        {
            public static readonly Spring DEFAULT = new Spring
            {
                NaturalFrequency = 4,
                Damping = 2,
                Response = 1.35f
            };

            public static readonly Spring DAMPED = new Spring
            {
                NaturalFrequency = 3,
                Damping = 2f,
                Response = 1.3f
            };

            public static Spring DampedWithStagger(float x) => new Spring
            {
                NaturalFrequency = 3 - x * 0.0025f,
                Damping = 2.5f,
                Response = 1.2f - x * 0.0001f
            };
        }
    }
}
