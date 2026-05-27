// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.Edit.Tools
{
    public class SliderPathVisualizer : CompositeDrawable
    {
        private readonly Slider slider;

        private readonly Container<Box> segments;
        private readonly Container<PathHandle> points;

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        public SliderPathVisualizer(Slider slider)
        {
            this.slider = slider;
            RelativeSizeAxes = Axes.Both;

            InternalChildren =
            [
                segments = new Container<Box>
                {
                    RelativeSizeAxes = Axes.Both,
                },
                points = new Container<PathHandle>
                {
                    RelativeSizeAxes = Axes.Both,
                }
            ];
        }

        protected override void Update()
        {
            base.Update();

            updatePath();
        }

        private void updatePath()
        {
            segments.Clear();
            points.Clear();

            float lineWidth = DrawWidth / ScreenSpaceDrawQuad.Width;

            var controlPoints = slider.Path.ControlPoints;

            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                var start = sliderToLocalSpace(controlPoints[i].Position);
                var end = sliderToLocalSpace(controlPoints[i + 1].Position);

                if (Precision.AlmostEquals(start, end))
                    continue;

                segments.Add(new Box
                {
                    Position = start,
                    Width = Vector2.Distance(start, end),
                    Height = lineWidth * 0.5f,
                    Origin = Anchor.CentreLeft,
                    Rotation = MathHelper.RadiansToDegrees(MathF.Atan2(end.Y - start.Y, end.X - start.X)),
                    EdgeSmoothness = new Vector2(0, 1f),
                    Alpha = 0.75f,
                });
            }

            foreach (var controlPoint in controlPoints)
            {
                points.Add(new PathHandle
                {
                    Position = sliderToLocalSpace(controlPoint.Position),
                    Colour = controlPoint.Type != null ? Color4.Red : Color4.White,
                });
            }
        }

        private Vector2 sliderToLocalSpace(Vector2 pathPosition)
        {
            var screenSpace = playfield.GamefieldToScreenSpace(slider.StackedPosition + pathPosition);

            return ToLocalSpace(screenSpace);
        }

        private partial class PathHandle : CompositeDrawable
        {
            public PathHandle()
            {
                Size = new Vector2(6);
                Origin = Anchor.Centre;

                InternalChildren =
                [
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = 0.3f,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(0.5f),
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                ];
            }
        }
    }
}
