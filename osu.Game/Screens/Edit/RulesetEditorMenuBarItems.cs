// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace osu.Game.Screens.Edit
{
    public abstract partial class RulesetEditorMenuBarItems : Component
    {
        public virtual IEnumerable<MenuItem> CreateFileItems() => Enumerable.Empty<MenuItem>();

        public virtual IEnumerable<MenuItem> CreateEditItems() => Enumerable.Empty<MenuItem>();

        public virtual IEnumerable<MenuItem> CreateViewItems() => Enumerable.Empty<MenuItem>();

        public virtual IEnumerable<MenuItem> CreateTimingItems() => Enumerable.Empty<MenuItem>();
    }
}
