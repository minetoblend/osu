// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace osu.Game.Screens.Edit.Commands
{
    public partial class EditorCommandHandler : Component
    {
        public void Submit(IEditorCommand command)
        {
            Debug.Assert(ActiveOperation == null, $"May not submit commands while there is an active {nameof(EditorOperation)}");

            recordUndoCommand(command.CreateUndo());
            Apply(command);
        }

        public void BeginOperation(EditorOperation operation)
        {
            if (ActiveOperation != null)
            {
                Logger.Log($"Found unfinished operation {ActiveOperation.GetType().Name} when starting new operation ${operation.GetType().Name}");
                FinishOperation();
            }

            ActiveOperation = operation;

            operation.CommandHandler = this;
            operation.Begin();
        }

        public bool FinishOperation()
        {
            if (ActiveOperation == null)
                return false;

            recordUndoCommand(ActiveOperation.CreateUndo());

            ActiveOperation = null;
            return true;
        }

        public bool CancelOperation()
        {
            if (ActiveOperation == null)
                return false;

            var undo = ActiveOperation.CreateUndo();
            Apply(undo);

            ActiveOperation = null;
            return true;
        }

        public bool Cancel()
        {
            if (ActiveOperation == null)
                return false;

            Apply(ActiveOperation.CreateUndo());
            redoStack.Clear();

            ActiveOperation = null;
            return true;
        }

        public EditorOperation? ActiveOperation { get; private set; }

        public bool Undo()
        {
            if (undoStack.TryPop(out var command))
            {
                redoStack.Push(command.CreateUndo());
                Apply(command);

                return true;
            }

            return false;
        }

        public bool Redo()
        {
            if (redoStack.TryPop(out var command))
            {
                undoStack.Push(command.CreateUndo());
                Apply(command);

                return true;
            }

            return false;
        }

        internal void Apply(IEditorCommand command) => command.Apply(commandContext);

        private CommandContext commandContext = null!;

        [BackgroundDependencyLoader]
        private void load(EditorBeatmap editorBeatmap)
        {
            commandContext = new CommandContext(
                editorBeatmap,
                Scheduler
            );
        }

        private void recordUndoCommand(IEditorCommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
        }

        private readonly CommandStack undoStack = new CommandStack(100);

        private readonly CommandStack redoStack = new CommandStack(100);

        public IBindable<bool> CanUndo => undoStack.CanPop;

        public IBindable<bool> CanRedo => undoStack.CanPop;
    }
}
