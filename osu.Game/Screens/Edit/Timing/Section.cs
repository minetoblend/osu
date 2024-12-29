// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Edit.Timing
{
    internal abstract partial class Section<T> : CompositeDrawable
        where T : ControlPoint
    {
        private Container content = null!;

        protected FillFlowContainer Flow { get; private set; } = null!;

        protected Bindable<IReadOnlyList<T>> ControlPoints { get; } = new Bindable<IReadOnlyList<T>>();

        private const float header_height = 50;

        [Resolved]
        protected EditorBeatmap Beatmap { get; private set; } = null!;

        [Resolved]
        protected ControlPointSelectionManager SelectionManager { get; private set; } = null!;

        [Resolved]
        protected IEditorChangeHandler? ChangeHandler { get; private set; }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colours)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeDuration = 200;
            AutoSizeEasing = Easing.OutQuint;
            AutoSizeAxes = Axes.Y;
            AlwaysPresent = true;

            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    Colour = colours.Background4,
                    RelativeSizeAxes = Axes.Both,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = header_height,
                    Padding = new MarginPadding { Horizontal = 10 },
                    Children = new Drawable[]
                    {
                        new OsuTextFlowContainer
                        {
                            Text = typeof(T).Name.Replace(nameof(Beatmaps.ControlPoints.ControlPoint), string.Empty)
                        }
                    }
                },
                content = new Container
                {
                    Y = header_height,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        Flow = new FillFlowContainer
                        {
                            Padding = new MarginPadding(10) { Top = 0 },
                            Spacing = new Vector2(20),
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                        },
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            SelectionManager.SelectionChanged += _ => Scheduler.AddOnce(selectionChanged);
            selectionChanged();

            ControlPoints.BindValueChanged(OnControlPointsChanged, true);
        }

        private void selectionChanged()
        {
            var pointsOfType = SelectionManager.Selection.OfType<T>().ToList();

            ControlPoints.Value = pointsOfType;

            if (pointsOfType.Count > 0)
            {
                content.BypassAutoSizeAxes = Axes.None;
                this.FadeIn(200);
            }
            else
            {
                content.BypassAutoSizeAxes = Axes.Y;
                this.FadeOut(200);
            }
        }

        protected abstract void OnControlPointsChanged(ValueChangedEvent<IReadOnlyList<T>> points);
    }
}
