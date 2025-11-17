// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match
{
    public class MatchmakingBackgroundScreen : BackgroundScreen
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(new Content { RelativeSizeAxes = Axes.Both });
        }

        public class Content : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load(TextureStore textures)
            {
                AddRangeInternal(new Drawable[]
                {
                    new BufferedContainer(cachedFrameBuffer: true)
                    {
                        RelativeSizeAxes = Axes.Both,
                        BlurSigma = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new Sprite
                            {
                                RelativeSizeAxes = Axes.Both,
                                Texture = textures.Get("Backgrounds/bg1"),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill,
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4Extensions.FromHex("#5B0039"),
                                Alpha = 0.4f
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black,
                                Alpha = 0.6f,
                            }
                        }
                    }
                });
            }
        }
    }
}
