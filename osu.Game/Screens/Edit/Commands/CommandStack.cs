// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace osu.Game.Screens.Edit.Commands
{
    public class CommandStack
    {
        private readonly int maxSize;

        private readonly List<IEditorCommand> entries = new List<IEditorCommand>();

        public CommandStack(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void Push(IEditorCommand command)
        {
            entries.Add(command);
            if (entries.Count > maxSize)
                entries.RemoveAt(0);

            sizeChanged();
        }

        public bool TryPop([MaybeNullWhen(false)] out IEditorCommand command)
        {
            if (entries.Count > 0)
            {
                command = entries[^1];
                entries.RemoveAt(entries.Count - 1);

                sizeChanged();

                return true;
            }

            command = null;
            return false;
        }

        private void sizeChanged() => canPop.Value = entries.Count > 0;

        public void Clear()
        {
            entries.Clear();
            sizeChanged();
        }

        public int Count => entries.Count;

        private readonly Bindable<bool> canPop = new BindableBool();

        public IBindable<bool> CanPop => canPop;
    }
}
