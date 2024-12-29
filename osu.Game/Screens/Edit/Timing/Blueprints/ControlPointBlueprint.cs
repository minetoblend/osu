// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Pooling;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class ControlPointBlueprint<T> : ControlPointBlueprint
        where T : ControlPoint
    {
        public new T ControlPoint => (T)base.ControlPoint;
    }

    public partial class ControlPointBlueprint : PoolableDrawableWithLifetime<ControlPointLifetimeEntry>
    {
        public ControlPointBlueprint()
        {
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
        }

        public readonly Bindable<bool> Selected = new BindableBool();

        public readonly Bindable<double> StartTimeBindable = new BindableDouble();
        public readonly Bindable<double> EndTimeBindable = new BindableDouble();

        protected ControlPointPiece ControlPointPiece { get; private set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(ControlPointPiece = CreateControlPointPiece());

            StartTimeBindable.BindValueChanged(_ => Scheduler.AddOnce(updatePosition));
            EndTimeBindable.BindValueChanged(_ => Scheduler.AddOnce(updatePosition));
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Selected.BindValueChanged(selected => ControlPointPiece.SelectionChanged(selected.NewValue), true);
        }

        protected virtual ControlPointPiece CreateControlPointPiece() => new DiamondControlPointPiece(this);

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        public ControlPoint ControlPoint => Entry!.Start;

        private void updatePosition()
        {
            double duration = Math.Min(EndTimeBindable.Value, editorClock.TrackLength) - StartTimeBindable.Value;

            X = (float)StartTimeBindable.Value;
            Width = (float)duration;
        }

        protected override void OnApply(ControlPointLifetimeEntry entry)
        {
            base.OnApply(entry);

            entry.Invalidated += onInvalidated;

            onInvalidated();
        }

        protected override void OnFree(ControlPointLifetimeEntry entry)
        {
            base.OnFree(entry);

            entry.Invalidated -= onInvalidated;
        }

        private void onInvalidated()
        {
            StartTimeBindable.Value = Entry!.LifetimeStart;
            EndTimeBindable.Value = Entry!.LifetimeEnd;
        }

        protected override bool ShouldBeAlive => true;
    }
}
