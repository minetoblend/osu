// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayCard
    {
        private partial class UserTagPill : CompositeDrawable
        {
            public required Drawable Background { get; init; }

            private readonly APITag tag;

            public UserTagPill(APITag tag)
            {
                this.tag = tag;

                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new CircularContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        Background.With(d =>
                        {
                            d.RelativeSizeAxes = Axes.Both;
                        }),
                        new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding
                            {
                                Horizontal = 8,
                                Vertical = 4
                            },
                            Text = tag.Name,
                            Font = OsuFont.Style.Caption1.With(weight: FontWeight.SemiBold)
                        }
                    },
                };
            }
        }
    }
}
