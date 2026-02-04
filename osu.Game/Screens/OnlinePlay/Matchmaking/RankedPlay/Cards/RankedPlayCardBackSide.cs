// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardBackSide : CompositeDrawable
    {
        public RankedPlayCardBackSide()
        {
            Size = RankedPlayCard.SIZE;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Masking = true;
            CornerRadius = RankedPlayCard.CORNER_RADIUS;
            BorderThickness = 3;
            BorderColour = Color4Extensions.FromHex("9F2C64");

            var texture = textures.Get("Online/RankedPlay/cardback");

            Debug.Assert(Precision.AlmostEquals((float)texture.Width / texture.Height, DrawWidth / DrawHeight));

            InternalChild = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Texture = texture,
            };
        }
    }
}
