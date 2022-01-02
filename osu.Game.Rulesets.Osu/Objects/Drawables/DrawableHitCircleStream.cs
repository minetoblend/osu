// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public class DrawableHitCircleStream : DrawableOsuHitObject
    {
        public Container CircleContainer;

        public DrawableHitCircleStream()
            : this(null)
        {
        }

        public DrawableHitCircleStream([CanBeNull] HitCircleStream s)
            : base(s)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                CircleContainer = new Container { RelativeSizeAxes = Axes.Both },
            };
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);
            CircleContainer.Add(hitObject);
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            CircleContainer.Clear(false);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) =>
            CircleContainer?.Children.Any(circle => circle.ReceivePositionalInputAt(screenSpacePos)) ?? base.ReceivePositionalInputAt(screenSpacePos);
    }
}
