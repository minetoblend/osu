// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Osu.Edit;
using osuTK;

namespace osu.Game.Tests.Visual.Editing
{
    public partial class TestSceneScaleGizmo : OsuTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new ScaleGizmo
            {
                Anchor = Anchor.Centre,
                Size = new Vector2(200),
            });
        }
    }
}
