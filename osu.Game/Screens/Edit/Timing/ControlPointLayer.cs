// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Localisation;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Screens.Edit.Timing.Blueprints;

namespace osu.Game.Screens.Edit.Timing
{
    [Cached(typeof(IPooledControlPointBlueprintProvider))]
    public abstract partial class ControlPointLayer<T, TDrawable> : TimelineLayer, IPooledControlPointBlueprintProvider
        where T : ControlPoint
        where TDrawable : ControlPointBlueprint, new()
    {
        protected ControlPointLayer(LocalisableString title)
            : base(title)
        {
        }

        [Resolved]
        private EditorBeatmap beatmap { get; set; } = null!;

        protected abstract IReadOnlyList<T> GetControlPointList(ControlPointInfo controlPointInfo);

        private ControlPointBlueprintContainer blueprintContainer = null!;

        private DrawablePool<TDrawable> pool = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(pool = new DrawablePool<TDrawable>(10, 40));

            Add(blueprintContainer = new ControlPointBlueprintContainer());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            foreach (var controlPoint in GetControlPointList(beatmap.ControlPointInfo))
                blueprintContainer.AddControlPoint(controlPoint);
        }

        public ControlPointBlueprint CreateBlueprintFor(ControlPointLifetimeEntry entry) => pool.Get(d => d.Entry = entry);
    }
}
