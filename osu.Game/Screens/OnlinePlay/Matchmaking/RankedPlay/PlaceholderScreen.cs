// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlaceholderScreen : RankedPlaySubScreen
    {
        public PlaceholderScreen(RankedPlayStage stage)
        {
            Add(new OsuSpriteText
            {
                Text = $"Placeholder screen: {stage.ToString()}",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = OsuFont.Style.Title,
            });
        }
    }
}
