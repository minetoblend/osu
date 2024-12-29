// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Screens.Edit.Timing.Blueprints;
using osuTK.Graphics;

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

            Padding = new MarginPadding { Vertical = 10 };

            SelectableAreaBackground background;

            AddRange(new Drawable[]
            {
                blueprintContainer = new ControlPointBlueprintContainer(),
                background = new SelectableAreaBackground(),
                background.CreateProxy().With(it => it.Depth = 1)
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            foreach (var controlPoint in GetControlPointList(beatmap.ControlPointInfo))
                blueprintContainer.AddControlPoint(controlPoint);

            beatmap.ControlPointInfo.ControlPointAdedd += controlPointAdded;
            beatmap.ControlPointInfo.ControlPointRemoved += controlPointRemoved;
        }

        private void controlPointAdded(ControlPoint controlPoint)
        {
            if (controlPoint is T)
                blueprintContainer.AddControlPoint(controlPoint);
        }

        private void controlPointRemoved(ControlPoint controlPoint)
        {
            if (controlPoint is T)
                blueprintContainer.RemoveControlPoint(controlPoint);
        }

        public ControlPointBlueprint CreateBlueprintFor(ControlPointLifetimeEntry entry) => pool.Get(d => d.Entry = entry);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            beatmap.ControlPointInfo.ControlPointAdedd += controlPointAdded;
            beatmap.ControlPointInfo.ControlPointRemoved += controlPointRemoved;
        }

        private partial class SelectableAreaBackground : CompositeDrawable
        {
            public SelectableAreaBackground()
            {
                RelativeSizeAxes = Axes.Both;
                Alpha = 0.05f;
                AlwaysPresent = true;
                AddRangeInternal(new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Horizontal = -200 },
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = 200,
                                Colour = ColourInfo.GradientHorizontal(Color4.Transparent, Color4.White)
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = 200,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Colour = ColourInfo.GradientHorizontal(Color4.White, Color4.Transparent)
                            }
                        }
                    }
                });
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.FadeTo(0.15f, 200);
                return false;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                this.FadeTo(0.05f, 200);
            }
        }
    }
}
