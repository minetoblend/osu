// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class DiamondControlPointPiece : ControlPointPiece
    {
        [Resolved]
        private TimelineLayer layer { get; set; } = null!;

        public DiamondControlPointPiece(ControlPointBlueprint blueprint)
            : base(blueprint)
        {
            Size = new Vector2(16);
            Rotation = 45;
        }

        private Container scaleContainer = null!;
        private Box outline = null!;
        private Box body = null!;

        protected override Drawable CreateContent() => scaleContainer = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 3,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = outline = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0.5f
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 1,
                    Scale = new Vector2(0.75f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = body = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                }
            },
        };

        internal override void SelectionChanged(bool selected)
        {
            base.SelectionChanged(selected);

            body.Colour = layer.LayerColour;

            if (selected)
            {
                outline.Colour = layer.LayerColour.Lighten(0.5f);
                outline.Alpha = 1;
            }
            else
            {
                outline.Colour = layer.LayerColour;
                outline.Alpha = 0.5f;
            }
        }

        protected override bool OnHover(HoverEvent e)
        {
            scaleContainer.ScaleTo(1.2f, 200, Easing.OutExpo);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            scaleContainer.ScaleTo(1f, 200, Easing.OutExpo);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            return true;
        }
    }
}
