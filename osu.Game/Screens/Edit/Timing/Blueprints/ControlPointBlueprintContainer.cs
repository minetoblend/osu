// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects.Pooling;
using osu.Game.Screens.Edit.Components.Timelines.Summary.Parts;
using osu.Game.Screens.Edit.Compose.Components.Timeline;

namespace osu.Game.Screens.Edit.Timing.Blueprints
{
    public partial class ControlPointBlueprintContainer : PooledDrawableWithLifetimeContainer<ControlPointLifetimeEntry, Drawable>
    {
        [Resolved]
        private Timeline timeline { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        [Resolved]
        private IPooledControlPointBlueprintProvider blueprintProvider { get; set; } = null!;

        private readonly List<ControlPointLifetimeEntry> lifetimeEntries = new List<ControlPointLifetimeEntry>();

        private TimelinePart blueprintContainer = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            var initialClock = Clock;

            Clock = editorClock;
            ProcessCustomClock = false;

            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                blueprintContainer = new TimelinePart
                {
                    RelativeSizeAxes = Axes.Both,
                    Clock = initialClock,
                    ProcessCustomClock = false,
                },
            };
        }

        protected override void AddDrawable(ControlPointLifetimeEntry entry, Drawable drawable) => blueprintContainer.Add(drawable);

        protected override void RemoveDrawable(ControlPointLifetimeEntry entry, Drawable drawable) => blueprintContainer.Remove(drawable, false);

        protected override Drawable GetDrawable(ControlPointLifetimeEntry entry) => blueprintProvider.CreateBlueprintFor(entry);

        public void AddControlPoint(ControlPoint controlPoint)
        {
            addEntry(controlPoint);
        }

        public void RemoveControlPoint(ControlPoint controlPoint)
        {
            removeEntry(controlPoint);
        }

        private void addEntry(ControlPoint controlPoint)
        {
            var newEntry = new ControlPointLifetimeEntry(controlPoint);

            int index = lifetimeEntries.AddInPlace(newEntry, Comparer<ControlPointLifetimeEntry>.Create((e1, e2) =>
            {
                int comp = e1.Start.Time.CompareTo(e2.Start.Time);

                if (comp != 0)
                    return comp;

                return -1;
            }));

            if (index < lifetimeEntries.Count - 1)
            {
                ControlPointLifetimeEntry nextEntry = lifetimeEntries[index + 1];
                newEntry.End = nextEntry.Start;
            }
            else
            {
                // The end point may be non-null during re-ordering
                newEntry.End = null;
            }

            if (index > 0)
            {
                // Update the previous control point's end point to the current control point

                ControlPointLifetimeEntry previousEntry = lifetimeEntries[index - 1];
                previousEntry.End = newEntry.Start;
            }

            Add(newEntry);
        }

        private void removeEntry(ControlPoint controlPoint)
        {
            int index = lifetimeEntries.FindIndex(e => ReferenceEquals(e.Start, controlPoint));

            var entry = lifetimeEntries[index];
            entry.UnbindEvents();

            lifetimeEntries.RemoveAt(index);
            Remove(entry);

            if (index > 0)
            {
                ControlPointLifetimeEntry previousEntry = lifetimeEntries[index - 1];
                previousEntry.End = entry.End;
            }
        }

        protected override void Update()
        {
            base.Update();

            PastLifetimeExtension = FutureLifetimeExtension = timeline.VisibleRange * 0.5f;
        }

        [Resolved]
        private ControlPointSelectionManager? selectionManager { get; set; }

        protected override bool OnClick(ClickEvent e)
        {
            selectionManager?.Clear();

            return false;
        }
    }
}
