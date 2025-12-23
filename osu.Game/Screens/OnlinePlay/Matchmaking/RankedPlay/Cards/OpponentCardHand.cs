// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    /// <summary>
    /// Card hand representing the opponent's current hand, intended to be placed at the top edge of the screen.
    /// </summary>
    public partial class OpponentCardHand : CardHand
    {
        protected override bool Flipped => true;
    }
}
