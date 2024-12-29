// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Compose.Components.Timeline;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Timing
{
    [Cached]
    public partial class LayeredTimeline : Container<TimelineLayer>
    {
        public LayeredTimeline()
        {
            RelativeSizeAxes = Axes.Both;

            layersFlow = new FillFlowContainer<TimelineLayer>
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(4),
            };
        }

        private readonly FillFlowContainer<TimelineLayer> layersFlow;

        protected override Container<TimelineLayer> Content => layersFlow;

        private const float header_height = 20;
        private const float header_width = 100;

        public Container HeaderContainer = null!;

        public Container BackgroundLayer = null!;

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            AddRangeInternal(new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = header_width },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Background2
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = header_height,
                            Colour = colourProvider.Background1,
                        },
                        new Timeline
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Height = header_height,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new HeaderTickDisplay { Y = 5 },
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Top = header_height },
                                    Children = new Drawable[]
                                    {
                                        BackgroundLayer = new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Name = "Background Layer"
                                        },
                                        new FullHeightTickDisplay(),
                                        layersFlow,
                                    }
                                }
                            }
                        },
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = header_width,
                    Masking = true,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 20,
                        Colour = Colour4.Black.MultiplyAlpha(0.1f),
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Background1
                        },
                        HeaderContainer = new Container
                        {
                            Padding = new MarginPadding { Top = header_height },
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
            });
        }

        private partial class HeaderTickDisplay : TimelineTickDisplay
        {
            protected override Vector2 GetSize(int indexInBar, int divisor)
            {
                if (indexInBar == 0)
                    return new Vector2(1.3f, 1);

                switch (divisor)
                {
                    case 1:
                    case 2:
                        return new Vector2(1, 0.8f);

                    case 3:
                    case 4:
                        return new Vector2(0.8f, 0.6f);

                    case 6:
                    case 8:
                        return new Vector2(0.8f, 0.4f);

                    default:
                        return new Vector2(0.8f, 0.4f);
                }
            }
        }

        private partial class FullHeightTickDisplay : TimelineTickDisplay
        {
            protected override Color4 GetColourFor(int divisor) => Color4.White;

            protected override float GetAlphaFor(int divisor) => divisor == 1 ? 0.15f : 0.075f;

            protected override Vector2 GetSize(int indexInBar, int divisor) => base.GetSize(indexInBar, divisor) with { Y = 1 };
        }
    }
}
