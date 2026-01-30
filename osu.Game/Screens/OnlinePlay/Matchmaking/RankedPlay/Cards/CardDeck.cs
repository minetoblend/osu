// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class CardDeck : CompositeDrawable
    {
        public Drawable TopLayer { get; private set; } = null!;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            AutoSizeAxes = Axes.Both;

            var texture = textures.Get("Online/RankedPlay/card-deck-bottom");
            Debug.Assert(texture != null);

            InternalChildren =
            [
                new Sprite
                {
                    Texture = texture,
                    Y = RankedPlayCard.SIZE.X - 18,
                    Width = RankedPlayCard.SIZE.Y,
                    Height = RankedPlayCard.SIZE.Y * texture.Height / texture.Width,
                },
                TopLayer = new RankedPlayCardBackSide
                {
                    Anchor = Anchor.TopRight,
                    Rotation = 90,
                },
            ];
        }
    }
}
