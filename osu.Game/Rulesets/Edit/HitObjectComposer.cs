// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Edit
{
    public abstract partial class HitObjectComposer<TObject> : HitObjectComposer
        where TObject : HitObject
    {
    }

    public abstract partial class HitObjectComposer : CompositeDrawable
    {
    }
}
