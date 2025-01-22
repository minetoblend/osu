// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Input.Bindings;

namespace osu.Game.Rulesets.Edit.Interactions
{
    public abstract partial class ComposeInteraction : CompositeDrawable, IKeyBindingHandler<GlobalAction>
    {
        [Resolved]
        private IInteractionContainer interactionContainer { get; set; } = null!;

        protected ComposeInteraction()
        {
            RelativeSizeAxes = Axes.Both;
        }

        #region Lifecycle

        protected void Complete() => interactionContainer.CompleteInteraction(this);

        protected void Cancel() => interactionContainer.CancelInteraction(this);

        internal virtual void OnComplete() => IsActive = false;

        internal virtual void OnCancel()
        {
            IsActive = false;
        }

        protected bool IsActive { get; private set; } = true;

        #endregion

        #region Input handling

        public override bool PropagatePositionalInputSubTree => IsActive && base.PropagatePositionalInputSubTree;

        public override bool PropagateNonPositionalInputSubTree => IsActive && base.PropagateNonPositionalInputSubTree;

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            switch (e.Action)
            {
                case GlobalAction.Select:
                    Complete();
                    return true;

                case GlobalAction.Back:
                    Cancel();
                    return false;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e) { }

        #endregion
    }
}
