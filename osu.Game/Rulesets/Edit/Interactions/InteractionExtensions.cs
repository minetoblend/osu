// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Edit.Interactions
{
    public static class InteractionExtensions
    {
        public static bool CompleteCurrentInteraction(this IInteractionContainer container)
        {
            var interaction = container.CurrentInteraction;

            if (interaction == null)
                return false;

            container.CompleteInteraction(interaction);
            return true;
        }

        public static bool CancelCurrentInteraction(this IInteractionContainer container)
        {
            var interaction = container.CurrentInteraction;

            if (interaction == null)
                return false;

            container.CancelInteraction(interaction);
            return true;
        }
    }
}
