// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;

namespace osu.Game.Screens.Edit.Timing
{
    public partial class EditorTimingAdjustButton : TimingAdjustButton
    {
        public EditorTimingAdjustButton(double adjustAmount)
            : base(adjustAmount) { }

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        protected override void ChangeBegan() => editorBeatmap.BeginChange();

        protected override void ChangeEnded() => editorBeatmap.EndChange();
    }
}
