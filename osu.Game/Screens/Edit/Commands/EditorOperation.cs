// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;

namespace osu.Game.Screens.Edit.Commands
{
    /// <summary>
    /// Represents an undoable action which exists over an extended period of time and can be updated.
    /// </summary>
    public abstract class EditorOperation : IEditorCommand
    {
        internal EditorCommandHandler CommandHandler = null!;

        public virtual void Begin() { }

        /// <summary>
        /// Finishes this operation and adds the resulting undo command to the undo stack.
        /// </summary>
        public virtual void Finish()
        {
            ensureIsActiveOperation();

            CommandHandler.FinishOperation();
        }

        /// <summary>
        /// Undoes the changes made by this command and removes it at the active operation
        /// </summary>
        public virtual void Cancel()
        {
            ensureIsActiveOperation();

            CommandHandler.CancelOperation();
        }

        public virtual bool TryFinish()
        {
            if (CommandHandler?.ActiveOperation == this)
            {
                Finish();
                return true;
            }

            return false;
        }

        public virtual bool TryCancel()
        {
            if (CommandHandler?.ActiveOperation == this)
            {
                Cancel();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Lets the command manager know that this command has updated & applies the new update
        /// </summary>
        public void Update()
        {
            ensureIsActiveOperation();

            CommandHandler.Apply(this);
        }

        public abstract void Apply();

        public abstract IEditorCommand CreateUndo();

        private void ensureIsActiveOperation()
        {
            Debug.Assert(CommandHandler != null, "Operation has not begun yet");
            Debug.Assert(CommandHandler.ActiveOperation == this, "Operation has ended");
        }
    }
}
