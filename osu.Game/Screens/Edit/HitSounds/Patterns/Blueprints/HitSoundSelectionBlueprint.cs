// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Edit.HitSounds.Patterns.Blueprints
{
    public partial class HitSoundSelectionBlueprint : HitObjectSelectionBlueprint<HitSound>
    {
        public HitSoundSelectionBlueprint(HitSound hitSound)
            : base(hitSound)
        {
            AddInternal(new Box
            {
                Size = new Vector2(40),
                Colour = Color4.Red,
            });
        }

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        protected ScrollingHitObjectContainer HitObjectContainer => ((ScrollingPlayfield)playfield).HitObjectContainer;

        protected override void Update()
        {
            base.Update();

            Position = Parent!.ToLocalSpace(DrawableObject.ScreenSpaceDrawQuad.TopLeft) - AnchorPosition;
            Width = HitObjectContainer.DrawWidth;

            Height = 30;
        }
    }
}
