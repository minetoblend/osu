// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent : CompositeDrawable
    {
        public readonly APIBeatmap Beatmap;

        private CardColours colours = null!;

        public RankedPlayCardContent(APIBeatmap beatmap)
        {
            Size = RankedPlayCard.SIZE;

            Beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Masking = true;
            CornerRadius = RankedPlayCard.CORNER_RADIUS;

            InternalChildren =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colours.Background,
                },
                new Container
                {
                    Name = "Top Area",
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Children =
                    [
                        new CardCover(Beatmap)
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    ]
                },
                new Container
                {
                    Name = "Bottom Area",
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = RankedPlayCard.SIZE.Y },
                    Children =
                    [
                    ]
                },
                new CardBorder()
            ];
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(colours = new CardColours(Beatmap, dependencies.Get<OsuColour>(), dependencies.Get<OverlayColourProvider>()));

            return dependencies;
        }

        private partial class CardBorder : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load(CardColours colours)
            {
                RelativeSizeAxes = Axes.Both;
                Masking = true;
                CornerRadius = RankedPlayCard.CORNER_RADIUS;
                BorderThickness = 1.5f;
                BorderColour = colours.Border;

                InternalChild = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true,
                    EdgeSmoothness = new Vector2(3),
                };
            }
        }
    }
}
