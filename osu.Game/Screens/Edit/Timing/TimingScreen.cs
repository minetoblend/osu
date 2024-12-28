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
    public partial class TimingScreen : EditorScreen
    {
        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }

        public Container HeaderContainer = null!;

        private const float header_height = 20;
        private const float header_width = 100;

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            AddRange(new Drawable[]
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
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = colourProvider.Background1,
                                        },
                                        new TimelineTickDisplay
                                        {
                                            Y = 5
                                        }
                                    }
                                },
                                new Container
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Top = header_height },
                                    Children = new Drawable[]
                                    {
                                        new TimingScreenTickDisplay(),
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
                            RelativeSizeAxes = Axes.Both
                        }
                    }
                },
            });
        }

        private partial class TimingScreenTickDisplay : TimelineTickDisplay
        {
            protected override Color4 GetColourFor(int divisor) => Color4.White;

            protected override float GetAlphaFor(int divisor) => divisor == 1 ? 0.15f : 0.075f;

            protected override Vector2 GetSize(int indexInBar, int divisor) => base.GetSize(indexInBar, divisor) with { Y = 1 };
        }
    }
}
