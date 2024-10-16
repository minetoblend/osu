// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Linq;
using osu.Game.Rulesets.Osu.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Screens.Edit.Commands;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit.Commands
{
    public class SetPositionCommand : IEditorCommand
    {
        private readonly OsuHitObject[] hitObjects;

        private readonly Vector2[] positions;

        private readonly Vector2[] initialPositions;

        public SetPositionCommand(OsuHitObject[] hitObjects, Vector2[] positions)
        {
            Debug.Assert(hitObjects.Length == positions.Length);

            this.hitObjects = hitObjects;
            this.positions = positions;

            initialPositions = hitObjects.Select(h => h.Position).ToArray();
        }

        public void Apply(CommandContext ctx)
        {
            for (int i = 0; i < hitObjects.Length; i++)
                hitObjects[i].Position = positions[i];

            // manually update stacking.
            // this intentionally bypasses the editor `UpdateState()` / beatmap processor flow for performance reasons,
            // as the entire flow is too expensive to run on every movement.
            ctx.Scheduler.AddOnce(OsuBeatmapProcessor.ApplyStacking, ctx.EditorBeatmap);
        }

        public IEditorCommand CreateUndo() => new SetPositionCommand(hitObjects, initialPositions);
    }
}
