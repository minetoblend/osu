// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Idle
{
    public partial class IdleScreen : MatchmakingSubScreen
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Padding = new MarginPadding { Right = -250 };

            InternalChild = new ProxiedParticipantList
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }
    }
}
