// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent
    {
        private class CardColours(APIBeatmap beatmap, OsuColour colour, OverlayColourProvider colourProvider)
        {
            private static readonly Color4 base_background = Color4Extensions.FromHex("#222228");

            public readonly Color4 Primary = colour.ForStarDifficulty(beatmap.StarRating);

            public readonly Color4 OnPrimary = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : colourProvider.Background5;

            public Colour4 Background
            {
                get
                {
                    Primary.ToHsl(out float hue, out float saturation, out _);

                    return base_background.Mix(Colour4.FromHSL(hue, saturation * 0.1f, 0.15f), 0.5f);
                }
            }

            public ColourInfo Border => ColourInfo.GradientVertical(
                Primary.Opacity(0.5f),
                Primary.Opacity(0f)
            );
        }
    }

    static file class Extensions
    {
        public static Color4 Mix(this Color4 lhs, Color4 rhs, float alpha) => new Color4(
            r: float.Lerp(lhs.R, rhs.R, alpha),
            g: float.Lerp(lhs.G, rhs.G, alpha),
            b: float.Lerp(lhs.B, rhs.B, alpha),
            a: float.Lerp(lhs.A, rhs.A, alpha)
        );

        public static void ToHsl(this Color4 color, out float hue, out float saturation, out float lightness)
        {
            var hsl = ((Colour4)color).ToHSL();

            hue = hsl.X;
            saturation = hsl.Y;
            lightness = hsl.Z;
        }
    }
}
