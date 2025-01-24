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
        private HitObjectComposer composer { get; set; } = null!;

        protected ComposeInteraction()
        {
            RelativeSizeAxes = Axes.Both;
        }

        #region Lifecycle

        internal virtual void OnComplete() => IsActive = false;

        internal virtual void OnCancel() => IsActive = false;

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
                    composer.CompleteInteraction(this);
                    return true;

                case GlobalAction.Back:
                    composer.CancelInteraction(this);
                    return false;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e) { }

        #endregion
    }
}
