// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Edit
{
    /// <summary>
    /// Top level container for editor compose mode.
    /// Responsible for providing snapping and generally gluing components together.
    /// </summary>
    /// <typeparam name="TObject">The base type of supported objects.</typeparam>
    public abstract partial class HitObjectComposer<TObject> : HitObjectComposer
        where TObject : HitObject
    {
    }

    /// <summary>
    /// A non-generic definition of a HitObject composer class.
    /// Generally used to access certain methods without requiring a generic type for <see cref="HitObjectComposer{T}" />.
    /// </summary>
    [Cached]
    public abstract partial class HitObjectComposer : CompositeDrawable
    {
    }
}
