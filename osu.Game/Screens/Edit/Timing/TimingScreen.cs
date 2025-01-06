// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps.ControlPoints;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class TimingScreen : EditorScreen
    {
        [Cached]
        public readonly Bindable<ControlPointGroup> SelectedGroup = new Bindable<ControlPointGroup>();

        [Resolved]
        private EditorClock? editorClock { get; set; }

        public TimingScreen()
            : base(EditorScreenMode.Timing)
        {
        }
    }
}
