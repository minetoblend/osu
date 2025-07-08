// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.Edit.Operators;

namespace osu.Game.Rulesets.Edit
{
    public partial class OperatorContainer : CompositeDrawable
    {
        private readonly Bindable<OperatorOverlay> currentOperator = new Bindable<OperatorOverlay>();

        public IBindable<OperatorOverlay> CurrentOperator => currentOperator;

        public void BeginOperator(OperatorOverlay op)
        {
            currentOperator.Value = op;
        }
    }
}
