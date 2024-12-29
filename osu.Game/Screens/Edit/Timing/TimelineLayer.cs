// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.Timing
{
    [Cached]
    public abstract partial class TimelineLayer : Container
    {
        protected TimelineLayer(LocalisableString title)
        {
            Title = title;
        }

        public readonly LocalisableString Title;

        public abstract Color4 LayerColour { get; }

        protected virtual float LayerHeight => 60;

        private TimelineLayerHeader header = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = LayerHeight;
        }

        [Resolved]
        private LayeredTimeline timeline { get; set; } = null!;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            timeline.HeaderContainer.Add(header = new TimelineLayerHeader(this));
        }

        protected override void Update()
        {
            base.Update();

            header.Y = Y;
            header.Height = Height;
        }
    }
}
