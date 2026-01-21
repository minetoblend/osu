// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent
    {
        private partial class CardMetadata(APIBeatmap beatmap) : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load(CardColours colours)
            {
                InternalChildren =
                [
                    new StarRatingBadge(beatmap)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Margin = new MarginPadding { Horizontal = 10, Top = 4 },
                    },
                ];
            }
        }

        private partial class StarRatingBadge(APIBeatmap beatmap) : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load(CardColours colours)
            {
                AutoSizeAxes = Axes.Y;
                Width = RankedPlayCard.SIZE.X - 20;

                Masking = true;
                CornerRadius = 2;

                InternalChildren =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = colours.Primary,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Horizontal = 3, Vertical = 1 },
                        Children =
                        [
                            new StarsDisplay(beatmap.StarRating)
                            {
                                StarSize = 6,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Colour = colours.OnPrimary,
                            },
                            new OsuSpriteText
                            {
                                Text = beatmap.StarRating.ToStandardFormattedString(maxDecimalDigits: 2),
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Font = OsuFont.GetFont(size: 10, weight: FontWeight.Bold),
                                Colour = colours.OnPrimary,
                            },
                        ]
                    }
                ];
            }
        }

        private partial class StarsDisplay(double starRating) : CompositeDrawable
        {
            public required float StarSize { get; init; }

            [BackgroundDependencyLoader]
            private void load()
            {
                AutoSizeAxes = Axes.Both;

                FillFlowContainer flow;

                InternalChild = flow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(2),
                };

                int numStars = (int)starRating - 1;

                for (int i = 0; i < numStars; i++)
                {
                    flow.Add(new SpriteIcon
                    {
                        Size = new Vector2(StarSize),
                        Icon = FontAwesome.Solid.Star,
                    });
                }

                float lastStarWidth = (int)((starRating % 1) * 4) / 4f;

                if (lastStarWidth > 0)
                {
                    flow.Add(new Container
                    {
                        Size = new Vector2(StarSize * lastStarWidth, StarSize),
                        Masking = true,
                        Child = new SpriteIcon
                        {
                            Icon = FontAwesome.Solid.Star,
                            Size = new Vector2(StarSize),
                        }
                    });
                }
            }
        }
    }
}
