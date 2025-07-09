// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Graphics.Containers;
using osuTK;

namespace osu.Game.Screens.Edit
{
    public partial class EditorScalingContainer : ScalingContainer.ScalingDrawSizePreservingFillContainer
    {
        public EditorScalingContainer()
            : base(true)
        {
            RelativeSizeAxes = Axes.Both;
        }

        protected override void Update()
        {
            float parentScaleInverse = Parent!.DrawInfo.MatrixInverse.ExtractScale().X;

            float scale = CurrentScale * parentScaleInverse;

            Scale = new Vector2(scale);
            Size = new Vector2(1 / scale);
        }
    }
}
