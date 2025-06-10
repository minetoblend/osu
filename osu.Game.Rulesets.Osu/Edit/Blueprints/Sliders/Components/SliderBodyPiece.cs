// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Skinning.Default;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders.Components
{
    public partial class SliderBodyPiece : BlueprintPiece<Slider>
    {
        private SkinnableDrawable<ManualSliderBody> skinnableBody = null!;

        private ManualSliderBody body => skinnableBody.Drawable;

        /// <summary>
        /// Offset in absolute (local) coordinates from the start of the curve.
        /// </summary>
        public Vector2 PathStartLocation => body.PathOffset;

        /// <summary>
        /// Offset in absolute (local) coordinates from the end of the curve.
        /// </summary>
        public Vector2 PathEndLocation => body.PathEndOffset;

        public SliderBodyPiece()
        {
            AutoSizeAxes = Axes.Both;

            // SliderSelectionBlueprint relies on calling ReceivePositionalInputAt on this drawable to determine whether selection should occur.
            // Without AlwaysPresent, a movement in a parent container (ie. the editor composer area resizing) could cause incorrect input handling.
            AlwaysPresent = true;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChild = skinnableBody = new SkinnableDrawable<ManualSliderBody>(new OsuSkinComponentLookup(OsuSkinComponents.SliderSelect),
                _ => new ManualSliderBody { BorderColour = colours.Yellow }, confineMode: ConfineMode.NoScaling)
            {
                RelativeSizeAxes = Axes.None,
                AutoSizeAxes = Axes.Both,
            };

            body.AccentColour = Color4.Transparent;
        }

        private int? lastVersion;

        public override void UpdateFrom(Slider hitObject)
        {
            base.UpdateFrom(hitObject);

            body.PathRadius = hitObject.Scale * OsuHitObject.OBJECT_RADIUS;

            if (lastVersion != hitObject.Path.Version.Value)
            {
                lastVersion = hitObject.Path.Version.Value;

                var vertices = new List<Vector2>();
                hitObject.Path.GetPathToProgress(vertices, 0, 1);

                body.SetVertices(vertices);
            }

            OriginPosition = body.PathOffset;
        }

        public void RecyclePath() => body.RecyclePath();

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => body.ReceivePositionalInputAt(screenSpacePos);
    }
}
