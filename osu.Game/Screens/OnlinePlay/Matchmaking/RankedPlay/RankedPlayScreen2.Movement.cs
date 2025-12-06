// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Utils;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen2
    {
        public static class MovementStyle
        {
            public static SpringParameters Energetic => new SpringParameters
            {
                NaturalFrequency = 3f,
                Damping = 0.9f,
                Response = 1.2f,
            };

            public static SpringParameters Smooth => new SpringParameters
            {
                NaturalFrequency = 2f,
                Damping = 1f,
                Response = 1.2f,
            };
        }
    }
}
