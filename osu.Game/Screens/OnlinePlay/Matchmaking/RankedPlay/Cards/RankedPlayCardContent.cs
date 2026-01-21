// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent : CompositeDrawable
    {
        public static readonly Vector2 SIZE = new Vector2(300, 500);

        public readonly APIBeatmap Beatmap;

        public RankedPlayCardContent(APIBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
    }
}
