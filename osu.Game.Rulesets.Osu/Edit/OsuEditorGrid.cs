// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Layout;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class OsuEditorGrid : CompositeDrawable
    {
        protected readonly LayoutValue GridCache = new LayoutValue(Invalidation.RequiredParentSizeToFit);

        private readonly Bindable<int> gridSize = new Bindable<int>(32);

        public OsuEditorGrid()
        {
            RelativeSizeAxes = Axes.Both;

            AddLayout(GridCache);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            createGrid();
        }

        protected override void Update()
        {
            base.Update();

            if (!GridCache.IsValid)
            {
                createGrid();
                GridCache.Validate();
            }
        }

        private void createGrid()
        {
            ClearInternal();

            addHorizontalLine(0f, 0.4f);
            addHorizontalLine(384f, 0.4f);
            addVerticalLine(0f, 0.4f);
            addVerticalLine(512f, 0.4f);
            addHorizontalLine(192f, 0.4f);
            addVerticalLine(256f, 0.4f);

            for (int i = gridSize.Value; i < DrawWidth; i += gridSize.Value)
            {
                if (!Precision.AlmostEquals(i, DrawWidth / 2))
                    addVerticalLine(i, 0.1f);
            }

            for (int i = gridSize.Value; i < DrawHeight; i += gridSize.Value)
            {
                if (!Precision.AlmostEquals(i, DrawHeight / 2))
                    addHorizontalLine(i, 0.1f);
            }
        }

        private void addHorizontalLine(float y, float alpha = 1f) => addLine(new Vector2(DrawWidth, y), new Vector2(0, y), alpha);
        private void addVerticalLine(float x, float alpha = 1f) => addLine(new Vector2(x, 0), new Vector2(x, DrawHeight), alpha);

        private void addLine(Vector2 start, Vector2 end, float alpha = 1f)
        {
            float lineWidth = DrawWidth / ScreenSpaceDrawQuad.Width;

            var direction = end - start;

            AddInternal(new Box
            {
                Position = start,
                Width = direction.Length,
                Height = lineWidth,
                Origin = Anchor.CentreLeft,
                Rotation = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Y, direction.X)),
                Alpha = alpha,
            });
        }
    }
}
