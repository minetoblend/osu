// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    [Cached(typeof(IScrollingInfo))]
    public partial class PatternEditor : CompositeDrawable, IScrollingInfo
    {
        private readonly HitSoundPattern pattern;

        private readonly BindableList<HitSoundLayer> layers;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        public PatternEditor(HitSoundPattern pattern)
        {
            this.pattern = pattern;

            layers = this.pattern.Layers.GetBoundCopy();

            RelativeSizeAxes = Axes.Both;
        }

        private PatternLayerContainer layerContainer = null!;
        private PatternPlayfield playfield = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                layerContainer,
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = PatternLayer.HEADER_WIDTH },
                    Child = new PatternBlueprintContainer(playfield)
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            playfield.Clock = editorClock;
            playfield.ProcessCustomClock = false;

            pattern.HitSoundAdded += Add;
            pattern.HitSoundRemoved += Remove;

            foreach (var h in pattern.HitSounds)
                playfield.Add(h);

            layers.BindCollectionChanged((_, _) => Scheduler.AddOnce(updateLayers));
        }

        public void Add(HitSound hitSound) => playfield.Add(hitSound);

        public void Remove(HitSound hitSound) => playfield.Remove(hitSound);

        private void updateLayers()
        {
            Dictionary<HitSound, int> previousLayers = new Dictionary<HitSound, int>();

            foreach (var h in pattern.HitSounds)
                previousLayers[h] = h.Layer;

            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];

                foreach (var (hitSound, _) in previousLayers.Where((pair) => pair.Value == layer.Index))
                    hitSound.Layer = i;

                layers[i].Index = i;
            }
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(pattern);
            dependencies.CacheAs<Playfield>(playfield = new PatternPlayfield());
            dependencies.CacheAs(layerContainer = new PatternLayerContainer(playfield)
            {
                RelativeSizeAxes = Axes.Both,
                Items = { BindTarget = pattern.Layers }
            });

            return dependencies;
        }

        #region IScrollingInfo

        public IBindable<ScrollingDirection> Direction { get; } = new Bindable<ScrollingDirection>(ScrollingDirection.Left);
        public IBindable<double> TimeRange { get; } = new Bindable<double>(4000);
        public IBindable<IScrollAlgorithm> Algorithm { get; } = new Bindable<IScrollAlgorithm>(new ConstantScrollAlgorithm());

        #endregion

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            pattern.HitSoundAdded -= Add;
            pattern.HitSoundRemoved -= Remove;
        }
    }
}
