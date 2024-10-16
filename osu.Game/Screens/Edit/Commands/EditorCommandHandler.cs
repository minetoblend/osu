// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;

namespace osu.Game.Screens.Edit.Commands
{
    public class EditorCommandHandler
    {
        public void Submit(IEditorCommand command)
        {
            record(command);
            apply(command);
        }

        private void apply(IEditorCommand command) => command.Apply();

        private void record(IEditorCommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
        }

        public bool Undo()
        {
            if (undoStack.TryPop(out var command))
            {
                redoStack.Push(command.CreateUndo());
                command.Apply();

                return true;
            }

            return false;
        }

        public bool Redo()
        {
            if (redoStack.TryPop(out var command))
            {
                undoStack.Push(command.CreateUndo());
                command.Apply();

                return true;
            }

            return false;
        }

        private readonly CommandStack undoStack = new CommandStack(100);

        private readonly CommandStack redoStack = new CommandStack(100);

        public IBindable<bool> CanUndo => undoStack.CanPop;

        public IBindable<bool> CanRedo => undoStack.CanPop;
    }
}
