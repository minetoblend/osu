// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingScreen : EditorScreen
    {
        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new LayeredTimeline
            {
                Children = new TimelineLayer[]
                {
                    new TimingPointLayer()
                }
            });
        }
    }
}
