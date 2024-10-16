// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit.Commands;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Commands
{
    public class MoveOperation : EditorOperation
    {
        private readonly OsuHitObject[] hitObjects;

        private readonly Vector2[] initialPositions;

        public Vector2 Delta;

        public MoveOperation(OsuHitObject[] hitObjects, Vector2 delta = new Vector2())
        {
            this.hitObjects = hitObjects;
            Delta = delta;

            initialPositions = hitObjects.Select(h => h.Position).ToArray();
        }

        public override void Apply(CommandContext ctx)
        {
            for (int i = 0; i < hitObjects.Length; i++)
                hitObjects[i].Position = initialPositions[i] + Delta;

            // manually update stacking.
            // this intentionally bypasses the editor `UpdateState()` / beatmap processor flow for performance reasons,
            // as the entire flow is too expensive to run on every movement.
            ctx.Scheduler.AddOnce(OsuBeatmapProcessor.ApplyStacking, ctx.EditorBeatmap);
        }

        public override IEditorCommand CreateUndo() => new SetPositionCommand(hitObjects, initialPositions);
    }
}
