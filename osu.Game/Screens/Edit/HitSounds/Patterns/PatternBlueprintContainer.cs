// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit.Compose.Components;
using osu.Game.Screens.Edit.HitSounds.Patterns.Blueprints;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class PatternBlueprintContainer : BlueprintContainer<HitObject>
    {
        [Resolved]
        private PatternLayerContainer layerContainer { get; set; } = null!;

        [Resolved]
        private HitSoundPattern pattern { get; set; } = null!;

        private readonly PatternPlayfield playfield;

        public PatternBlueprintContainer(PatternPlayfield playfield)
        {
            this.playfield = playfield;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            pattern.HitSoundAdded += AddBlueprintFor;
            pattern.HitSoundRemoved += RemoveBlueprintFor;

            foreach (var h in pattern.HitSounds)
                AddBlueprintFor(h);
        }

        protected override SelectionHandler<HitObject> CreateSelectionHandler() => new PatternSelectionHandler();

        protected override DragBox CreateDragBox() => new ScrollingDragBox(playfield);

        protected override SelectionBlueprint<HitObject>? CreateBlueprintFor(HitObject item)
        {
            var drawable = playfield.HitObjectContainer.AliveObjects.FirstOrDefault(d => d.HitObject == item);

            if (item is HitSound hitSound)
            {
                return new HitSoundSelectionBlueprint(hitSound)
                {
                    DrawableObject = drawable
                };
            }

            return base.CreateBlueprintFor(item);
        }

        protected override void SelectAll()
        {
            // TODO
        }

        protected override bool TryMoveBlueprints(DragEvent e, IList<(SelectionBlueprint<HitObject> blueprint, Vector2[] originalSnapPositions)> blueprints)
        {
            return true;
        }
    }
}
