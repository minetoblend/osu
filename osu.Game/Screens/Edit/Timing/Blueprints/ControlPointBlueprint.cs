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
    public partial class ControlPointBlueprint : PoolableDrawableWithLifetime<ControlPointLifetimeEntry>
    {
        public ControlPointBlueprint()
        {
            RelativePositionAxes = Axes.Both;
            RelativeSizeAxes = Axes.Both;
        }

        public readonly Bindable<double> StartTimeBindable = new BindableDouble();
        public readonly Bindable<double> EndTimeBindable = new BindableDouble();

        [BackgroundDependencyLoader]
        private void load()
        {
            StartTimeBindable.BindValueChanged(_ => Scheduler.AddOnce(updatePosition));
            EndTimeBindable.BindValueChanged(_ => Scheduler.AddOnce(updatePosition));
        }

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
