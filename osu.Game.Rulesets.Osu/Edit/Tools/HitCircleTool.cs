// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit.Tools
{
    public class HitCircleTool : OsuHitObjectPlacementTool<HitCircle>
    {
        public HitCircleTool()
            : base(new HitCircle())
        {
        }

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        protected override void UpdateTimeAndPosition(Vector2 position, double time)
        {
            // once mouse-down is pressed the start time should no longer change
            if (State != PlacementState.Active)
                HitObject.StartTime = editorClock.CurrentTime;

            HitObject.Position = SnapProvider.SnapToHitObjects(position, exclude: [HitObject])?.Position ?? position;
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left)
                return false;

            BeginPlacement();
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Left)
                EndPlacement(true);
        }

        protected override void OnEndPlacement(bool didCommit)
        {
            if (!didCommit)
                return;

            var overlapping = editorBeatmap.HitObjects
                                           .Where(it => it != HitObject && Precision.AlmostEquals(it.StartTime, HitObject.StartTime, 1))
                                           .ToArray();

            foreach (var h in overlapping)
                editorBeatmap.Remove(h);
        }
    }
}
