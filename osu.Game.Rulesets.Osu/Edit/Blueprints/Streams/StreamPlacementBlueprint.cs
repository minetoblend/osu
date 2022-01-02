// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Rulesets.Osu.Edit.Blueprints.HitCircles.Components;
using osu.Game.Rulesets.Osu.Edit.Blueprints.Sliders.Components;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Edit.Blueprints.Streams
{
    public class StreamPlacementBlueprint : PathPlacementBlueprint
    {
        public new Objects.HitCircleStream HitObject => (Objects.HitCircleStream)base.HitObject;

        private Container<HitCirclePiece> circleContainer;

        public StreamPlacementBlueprint()
            : base(new Objects.HitCircleStream())
        {
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChildren = new Drawable[]
            {
                circleContainer = new Container<HitCirclePiece> { RelativeSizeAxes = Axes.Both },
                ControlPointVisualiser = new PathControlPointVisualiser(HitObject, false)
            };

            SetState(PathPlacementState.Initial);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            HitObject.BeatDivisor = editorBeatmap.BeatDivisor;
        }

        protected override void UpdateHitObject()
        {
            base.UpdateHitObject();

            editorBeatmap?.Update(HitObject);

            circleContainer.Clear();

            foreach (var circle in HitObject.NestedHitObjects.OfType<HitCircle>())
            {
                var piece = new HitCirclePiece();
                circleContainer.Add(piece);
                piece.UpdateFrom(circle);
            }
        }
    }
}
