// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimelineLayerHeader : CompositeDrawable
    {
        public TimelineLayerHeader(TimelineLayer layer)
        {
            RelativeSizeAxes = Axes.X;
            Padding = new MarginPadding(6);

            AddInternal(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(4),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Padding = new MarginPadding { Vertical = 3 },
                        Child = new FastCircle
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 3,
                            Colour = layer.LayerColour
                        },
                    },
                    new OsuSpriteText
                    {
                        Text = layer.Title
                    }
                }
            });
        }
    }
}
