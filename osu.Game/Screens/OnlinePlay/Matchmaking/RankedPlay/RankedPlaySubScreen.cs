// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public abstract partial class RankedPlaySubScreen : CompositeDrawable
    {
        protected RankedPlaySubScreen()
        {
            RelativeSizeAxes = Axes.Both;
        }
    }
}
