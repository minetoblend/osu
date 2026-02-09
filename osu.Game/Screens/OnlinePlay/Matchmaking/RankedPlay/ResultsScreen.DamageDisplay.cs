// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        private partial class DamageDisplay(RankedPlayDamageInfo damageInfo) : CompositeDrawable
        {
            public Container Background { get; private set; } = null!;

            public ScoreCounter DamageCounter { get; private set; } = null!;

            public OsuSpriteText MultiplierText { get; private set; } = null!;

            [BackgroundDependencyLoader]
            private void load(TextureStore textures, RankedPlayMatchInfo matchInfo, OsuColour colour)
            {
                int numDigits = (int)Math.Ceiling(Math.Log10(damageInfo.Damage));

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
                                Texture = textures.Get("Online/RankedPlay/damage-display-background"),
                                TextureInsetRelativeAxes = Axes.None,
                                TextureInset = new MarginPadding { Horizontal = 30 },
                                Colour = Color4Extensions.FromHex("222228"),
                            },
                            new NineSliceSprite
                            {
                                RelativeSizeAxes = Axes.Both,
                                Texture = textures.Get("Online/RankedPlay/damage-display-border"),
                                TextureInsetRelativeAxes = Axes.None,
                                TextureInset = new MarginPadding { Horizontal = 30 },
                                Alpha = 0.25f,
                                Colour = Color4Extensions.FromHex("ddddff")
                            },
                        ]
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children =
                        [
                            DamageCounter = new ScoreCounter(numDigits)
                            {
                                Font = OsuFont.GetFont(size: 36, weight: FontWeight.SemiBold, fixedWidth: true),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Alpha = 1,
                                Spacing = new Vector2(-2),
                            },
                            MultiplierText = new OsuSpriteText
                            {
                                BypassAutoSizeAxes = Axes.Both,
                                Text = $"{matchInfo.RoomState.DamageMultiplier.ToStandardFormattedString(maxDecimalDigits: 1)}x",
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.Centre,
                                Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 42),
                                Rotation = 30,
                                Alpha = 0,
                                Colour = colour.RedLight
                            }
                        ]
                    },

                    new OsuSpriteText
                    {
                        Text = "Damage",
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.Centre,
                        Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 22),
                    },
                ];
            }
        }
    }
}
