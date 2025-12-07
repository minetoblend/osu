// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public class DebugBox : Container
    {
        [Resolved(name: "debugEnabled")]
        private Bindable<bool>? debugEnabled { get; set; }

        public DebugBox()
        {
            RelativeSizeAxes = Axes.Both;

            Masking = true;
            MaskingSmoothness = 1;

            BorderColour = Color4.Red;
            BorderThickness = 1.5f;
            Padding = new MarginPadding(-1);
            Alpha = 0;
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0.05f,
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            // LoadComplete won't get called if the drawable is hidden so doing this in BDL instead
            debugEnabled?.BindValueChanged(e => Alpha = e.NewValue ? 1 : 0, true);
        }
    }
}
