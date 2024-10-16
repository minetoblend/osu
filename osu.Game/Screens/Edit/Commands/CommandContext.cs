// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Threading;

namespace osu.Game.Screens.Edit.Commands
{
    /// <summary>
    /// Provides commands access to various resources required to perform their actions.
    /// </summary>
    public class CommandContext
    {
        public readonly Scheduler Scheduler;

        public readonly EditorBeatmap EditorBeatmap;

        public CommandContext(EditorBeatmap editorBeatmap, Scheduler scheduler)
        {
            Scheduler = scheduler;
            EditorBeatmap = editorBeatmap;
        }
    }
}
