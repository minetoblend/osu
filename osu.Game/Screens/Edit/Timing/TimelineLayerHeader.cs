// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimelineLayerHeader : CompositeDrawable
    {
        public TimelineLayerHeader(TimelineLayer layer)
        {
            RelativeSizeAxes = Axes.X;
            Padding = new MarginPadding(6);

            AddInternal(new OsuSpriteText
            {
                Text = layer.Title,
                Font = OsuFont.GetFont(weight: FontWeight.SemiBold)
            });
        }
    }
}
