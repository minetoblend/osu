using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Framework.Localisation;

namespace osu.Game.Overlays.Settings.Sections.Toy
{
    internal partial class IntifaceSettings : SettingsSubsection
    {
        protected override LocalisableString Header => "Intiface";

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsTextBox
                {
                    LabelText = "Intiface address (restart required)",
                    Current = config.GetBindable<string>(OsuSetting.IntifaceAddress)
                }
            };
        }
    }
}
