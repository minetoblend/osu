// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayCard
    {
        private partial class DetailsPill : CompositeDrawable
        {
            public required Drawable Background { get; init; }

            private readonly LocalisableString name;
            private readonly LocalisableString value;

            public DetailsPill(LocalisableString name, LocalisableString value)
            {
                this.name = name;
                this.value = value;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        Background.With(d =>
                        {
                            d.RelativeSizeAxes = Axes.Both;
                        }),
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(2),
                            Children = new Drawable[]
                            {
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = name,
                                    Font = OsuFont.Style.Caption1.With(weight: FontWeight.Bold),
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = value,
                                    Font = OsuFont.Style.Caption1,
                                    Colour = Colour4.FromHex("#F0DBEB")
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
