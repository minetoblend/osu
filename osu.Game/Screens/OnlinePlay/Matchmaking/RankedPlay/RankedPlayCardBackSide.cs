// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayCardBackSide : CompositeDrawable
    {
        public RankedPlayCardBackSide()
        {
            Size = RankedPlayCard.SIZE;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = colourProvider.Background1,
            };
        }
    }
}
