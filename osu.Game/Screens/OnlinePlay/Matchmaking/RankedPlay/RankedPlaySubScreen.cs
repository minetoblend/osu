// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Globalization;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.Multiplayer;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public abstract partial class RankedPlaySubScreen : Container
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        protected MultiplayerClient Client => client;

        public virtual double CardTransitionStagger => 0;

        protected override Container<Drawable> Content { get; }

        protected readonly Container CenterColumn;

        protected readonly FillFlowContainer ButtonsContainer;

        protected RankedPlaySubScreen()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren =
            [
                CenterColumn = new Container
                {
                    Name = "Center Column",
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding(20),
                },
                Content = new Container
                {
                    Name = "Content",
                    RelativeSizeAxes = Axes.Both,
                },
                ButtonsContainer = new FillFlowContainer
                {
                    Name = "Buttons",
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 30,
                    Y = -110,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(8)
                },
            ];
        }

        protected override void Update()
        {
            base.Update();

            CenterColumn.Width = DrawWidth - RankedPlayCornerPiece.WidthFor(DrawWidth) * 2;
        }

        public virtual void OnEntering(RankedPlaySubScreen? previous)
        {
        }

        public virtual void OnExiting(RankedPlaySubScreen? next)
        {
            Hide();
        }

        public virtual void CardAdded(RankedPlayScreen.Card card, CardOwner owner)
        {
        }

        public virtual void CardRemoved(RankedPlayScreen.Card card, CardOwner owner)
        {
        }

        public virtual void CardPlayed(RankedPlayScreen.Card card)
        {
        }

        public virtual ICardFacadeContainer? PlayerCardContainer => null;

        public virtual ICardFacadeContainer? OpponentCardContainer => null;

        protected static string FormatRoundIndex(int roundIndex)
        {
            int roundNumber = roundIndex + 1;

            return roundNumber >= 10 ? roundNumber.Ordinalize(CultureInfo.InvariantCulture) : roundNumber.ToOrdinalWords(CultureInfo.InvariantCulture);
        }
    }
}
