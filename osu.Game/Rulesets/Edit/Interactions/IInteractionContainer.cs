// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Edit.Interactions
{
    public interface IInteractionContainer
    {
        public ComposeInteraction? CurrentInteraction { get; }

        public void BeginInteraction(ComposeInteraction interaction);

        public bool CompleteInteraction(ComposeInteraction interaction);

        public bool CancelInteraction(ComposeInteraction interaction);
    }
}
