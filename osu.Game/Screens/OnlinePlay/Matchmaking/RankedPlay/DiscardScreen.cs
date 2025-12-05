// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class DiscardScreen(RankedPlayCardItem[] initialCards) : RankedPlaySubScreen
    {
        private OsuButton discardButton = null!;
        private OsuSpriteText readyToGo = null!;
        private OsuTextFlowContainer explainer = null!;

        [Resolved]
        private PlayerCardHand playerCards { get; set; } = new PlayerCardHand();

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.25f, 0.5f),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    Children = new[]
                    {
                        discardButton = new RoundedButton
                        {
                            Text = "Discard",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(100, 40),
                            Action = playDiscardSequence,
                            Name = "Discard"
                        },
                    }
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.5f,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Top = 50 },
                    Spacing = new Vector2(10),
                    Children =
                    [
                        new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "Discarding Phase",
                            Font = OsuFont.Style.Title
                        },
                        readyToGo = new OsuSpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "You’re ready to go!",
                            Font = OsuFont.Style.Subtitle,
                            Colour = Color4Extensions.FromHex("#47B4EC"),
                            Alpha = 0,
                        },
                        explainer = new OsuTextFlowContainer(s => s.Font = OsuFont.Style.Heading2)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            TextAnchor = Anchor.TopCentre,
                            Margin = new MarginPadding { Top = 50 },
                            ParagraphSpacing = 1,
                            Alpha = 0,
                        }.With(d =>
                        {
                            d.AddParagraph("These are your Cards for this match!");
                            d.AddParagraph("When it’s your pick, you can choose one card to go head-to-head with against your opponent!");
                        })
                    ]
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            playerCards.Clear();
            foreach (var card in initialCards)
                playerCards.AddCard(card);

            playerCards.State = PlayerCardHand.CardState.Hidden;
            playerCards.ApplyLayoutImmediately();
            playerCards.State = PlayerCardHand.CardState.Hand;

            playerCards.AllowSelection.Value = true;
        }

        private void playDiscardSequence()
        {
            discardButton.Hide();

            playerCards.AllowSelection.Value = false;

            // TODO: use list from server
            playerCards.DiscardSelectedCards();

            double delay = 2000;

            for (int i = playerCards.Cards.Count(); i < 5; i++)
            {
                Scheduler.AddDelayed(() =>
                {
                    playerCards.DrawCard(new RankedPlayCardItem());
                }, delay);

                delay += 100;
            }

            delay += 2000;

            readyToGo.Delay(delay).FadeIn(100);
            explainer.Delay(delay).FadeIn(100);

            Scheduler.AddDelayed(() => playerCards.State = PlayerCardHand.CardState.Lineup, delay);
        }
    }
}
