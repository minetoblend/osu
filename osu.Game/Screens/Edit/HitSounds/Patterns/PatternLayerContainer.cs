// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Graphics.Containers;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class PatternLayerContainer : RearrangeableListContainer<HitSoundLayer>
    {
        public event Action? LayersChanged;

        private readonly PatternPlayfield playfield;

        [Cached]
        private readonly Bindable<HitSoundLayer?> draggedLayer = new Bindable<HitSoundLayer?>();

        public PatternLayerContainer(PatternPlayfield playfield)
        {
            this.playfield = playfield;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            ScrollContainer.Add(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = PatternLayer.HEADER_WIDTH },
                Child = playfield,
            });
        }

        protected override ScrollContainer<Drawable> CreateScrollContainer() => new PatternLayerScrollContainer();

        protected override RearrangeableListItem<HitSoundLayer> CreateDrawable(HitSoundLayer item) => new PatternLayer(item);

        protected override FillFlowContainer<RearrangeableListItem<HitSoundLayer>> CreateListFillFlowContainer() =>
            base.CreateListFillFlowContainer().With(c =>
            {
                c.LayoutDuration = 200;
                c.LayoutEasing = Easing.OutExpo;
            });

        protected override void OnItemsChanged()
        {
            base.OnItemsChanged();

            LayersChanged?.Invoke();
        }

        public PatternLayer GetLayer(int index)
        {
            return (ListContainer.Children[index] as PatternLayer)!;
        }

        private partial class PatternLayerScrollContainer : OsuScrollContainer
        {
            protected override bool OnScroll(ScrollEvent e) => false;

            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) =>
                ToLocalSpace(screenSpacePos).X < PatternLayer.HEADER_WIDTH;
        }
    }
}
