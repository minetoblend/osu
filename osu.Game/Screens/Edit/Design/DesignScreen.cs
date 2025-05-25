// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Screens.Edit.Design
{
    public partial class DesignScreen : EditorScreen
    {
        private readonly ScreenWhiteBox.UnderConstructionMessage message;

        public DesignScreen()
            : base(EditorScreenMode.Design)
        {
            Child = message = new ScreenWhiteBox.UnderConstructionMessage("Design mode")
            {
                AutoPlayAnimation = false
            };
        }

        protected override void PopIn()
        {
            base.PopIn();

            message.PlayEntryAnimation();
        }
    }
}
