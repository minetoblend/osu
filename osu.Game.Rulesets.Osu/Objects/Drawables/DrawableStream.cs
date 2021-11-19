// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Objects.Drawables
{
    public class DrawableStream : DrawableOsuHitObject
    {
        public new StreamHitObject HitObject => (StreamHitObject)base.HitObject;

        private Container<DrawableHitCircle> circleContainer;

        protected DrawableStream(OsuHitObject hitObject)
            : base(hitObject)
        {
        }

        public override bool DisplayResult => true;

        public DrawableStream()
            : this(null)
        {
        }

        public DrawableStream([CanBeNull] StreamHitObject s = null)
            : base(s)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                circleContainer = new Container<DrawableHitCircle> { RelativeSizeAxes = Axes.Both },
            };
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            if (hitObject is DrawableHitCircle circle)
            {
                circleContainer.Add(circle);
            }
        }



        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();

            circleContainer.Clear(false);
        }

        protected override void LoadSamples()
        {
            base.LoadSamples();
        }
    }
}
