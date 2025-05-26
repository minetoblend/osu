// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class DrawableHitSound : DrawableHitObject<HitSound>
    {
        [Resolved]
        private PatternLayerContainer layerContainer { get; set; } = null!;

        private readonly Cached<PatternLayer> layer = new Cached<PatternLayer>();

        private readonly Bindable<int> layerIndexBindable = new Bindable<int>();

        public DrawableHitSound()
            : base(null)
        {
            Size = new Vector2(40);
            AddInternal(new Box { RelativeSizeAxes = Axes.Both });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            layerIndexBindable.BindValueChanged(_ => layersChanged());

            layerContainer.LayersChanged += layersChanged;
        }

        protected override void Update()
        {
            base.Update();

            if (!layer.IsValid)
                layer.Value = layerContainer.GetLayer(HitObject.Layer);

            Y = layer.Value.Y;
            Height = layer.Value.DrawHeight;
        }

        private void layersChanged() => layer.Invalidate();

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset >= 0)
                ApplyMaxResult();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            if (state == ArmedState.Hit)
                this.FadeOut();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            layerContainer.LayersChanged -= layersChanged;
        }
    }
}
