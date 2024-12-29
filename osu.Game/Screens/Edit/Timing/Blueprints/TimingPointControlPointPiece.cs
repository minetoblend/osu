// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Screens.Edit.Compose.Components.Timeline;
using osuTK;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class TimingPointControlPointPiece : ControlPointPiece
    {
        public TimingPointControlPointPiece(ControlPointBlueprint blueprint)
            : base(blueprint)
        {
            Width = 10;
            RelativeSizeAxes = Axes.Y;
        }

        private Container content = null!;

        protected override Drawable CreateContent() => content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 2,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                new Box
                {
                    Size = new Vector2(1f / (float)Math.Sqrt(2)),
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.Centre,
                    Rotation = 45,
                },
                new Box
                {
                    Size = new Vector2(1f / (float)Math.Sqrt(2)),
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.Centre,
                    Rotation = 45,
                },
            }
        };

        protected override bool EnableSnapping(UIEvent e) => false;

        [Resolved]
        private Timeline? timeline { get; set; }

        [Resolved]
        private TimelineLayer layer { get; set; } = null!;

        internal override void SelectionChanged(bool selected)
        {
            base.SelectionChanged(selected);

            content.Colour = selected ? layer.LayerColour.Lighten(0.5f) : layer.LayerColour;
        }

        protected override void OnDrag(DragEvent e)
        {
            base.OnDrag(e);

            timeline?.InvalidateTicks();
        }
    }
}
