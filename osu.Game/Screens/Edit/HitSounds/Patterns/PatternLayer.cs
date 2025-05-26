// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    [Cached]
    public partial class PatternLayer : RearrangeableListItem<HitSoundLayer>
    {
        public const float HEADER_WIDTH = 190;

        private PatternLayerHeader header = null!;

        public Bindable<bool> Enabled = new Bindable<bool>(true);

        [Resolved]
        private Bindable<HitSoundLayer?> draggedLayer { get; set; } = null!;

        public PatternLayer(HitSoundLayer item)
            : base(item)
        {
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    Name = "Pattern Header",
                    RelativeSizeAxes = Axes.Y,
                    Width = HEADER_WIDTH,
                    Child = header = new PatternLayerHeader(Model),
                },
                new Container
                {
                    Name = "Pattern Timeline",
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = HEADER_WIDTH },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Height = 16,
                            Colour = colourProvider.Background5
                        }
                    }
                }
            };
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            if (!base.OnDragStart(e))
                return false;

            draggedLayer.Value = Model;

            return true;
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            base.OnDragEnd(e);

            if (draggedLayer.Value == Model)
                draggedLayer.Value = null;
        }

        protected override bool IsDraggableAt(Vector2 screenSpacePos) => header.HandlingDrag;
    }
}
