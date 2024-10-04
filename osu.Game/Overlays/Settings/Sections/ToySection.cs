using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Overlays.Settings.Sections.Toy;
using osu.Framework.Localisation;

namespace osu.Game.Overlays.Settings.Sections
{
    internal partial class ToySection : SettingsSection
    {
        public override LocalisableString Header => "Toy";

        public override Drawable CreateIcon() => new SpriteIcon
        {
            Icon = FontAwesome.Solid.GrinTongue
        };

        public ToySection()
        {
            Children = new Drawable[]
            {
                new IntifaceSettings(),
            };
        }
    }
}
