// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit.Interactions;

namespace osu.Game.Rulesets.Osu.Edit.Interactions
{
    public partial class OsuRotateInteraction : ComposeInteraction, IRequiresSelectionBox
    {
        public bool CanScale => false;
        public bool CanRotate => true;
    }
}
