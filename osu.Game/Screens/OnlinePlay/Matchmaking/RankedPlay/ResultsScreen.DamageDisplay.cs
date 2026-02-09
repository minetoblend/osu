// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        private partial class DamageDisplay : CompositeDrawable
        {
            public Container Background { get; private set; } = null!;

            public ScoreCounter DamageCounter { get; private set; } = null!;

            [BackgroundDependencyLoader]
            private void load(TextureStore textures, RankedPlayMatchInfo matchInfo)
            {
                InternalChildren =
                [
                    Background = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children =
                        [
                            new NineSliceSprite
                            {
                                RelativeSizeAxes = Axes.Both,
                                Texture = textures.Get("Online/RankedPlay/hexagon"),
                                TextureInsetRelativeAxes = Axes.None,
                                TextureInset = new MarginPadding { Horizontal = 30 },
                                Colour = Color4Extensions.FromHex("222228"),
                            },
                            new NineSliceSprite
                            {
                                RelativeSizeAxes = Axes.Both,
                                Texture = textures.Get("Online/RankedPlay/hexagon-border"),
                                TextureInsetRelativeAxes = Axes.None,
                                TextureInset = new MarginPadding { Horizontal = 30 },
                                Alpha = 0.25f,
                                Colour = Color4Extensions.FromHex("ddddff")
                            },
                        ]
                    },
                    DamageCounter = new ScoreCounter(7)
                    {
                        Font = OsuFont.GetFont(size: 36, weight: FontWeight.SemiBold, fixedWidth: true),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 1,
                        Spacing = new Vector2(-2),
                    },
                    new OsuSpriteText
                    {
                        Text = $"Damage {matchInfo.RoomState.DamageMultiplier.ToStandardFormattedString(maxDecimalDigits: 1)}x",
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 18),
                    }
                ];
            }
        }
    }
}
