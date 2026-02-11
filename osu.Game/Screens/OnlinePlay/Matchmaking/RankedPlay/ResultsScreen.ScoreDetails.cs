// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Scoring;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class ResultsScreen
    {
        public partial class ScoreDetails(ScoreInfo score, RankedPlayColourScheme colourScheme) : CompositeDrawable
        {
            public ScoreCounter Counter { get; private set; } = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions =
                    [
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    ],
                    Content = new Drawable[][]
                    {
                        [],
                        [
                            Counter = new ScoreCounter
                            {
                                Font = OsuFont.GetFont(size: 60, fixedWidth: true),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Spacing = new Vector2(-4),
                                Alpha = 0,
                            }
                        ]
                    }
                };
            }
        }
    }
}
