// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Online.Rooms;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class DiscardScreen(MultiplayerPlaylistItem[] hand) : RankedPlaySubScreen
    {
        private Container<Card> cardFlow = null!;

        private OsuButton discardButton = null!;
        private OsuButton skipButton = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                cardFlow = new Container<Card>
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.5f),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Children = hand.Select(item => new Card(item)
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Clicked = cardClicked
                    }).ToArray(),
                },
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
                            Action = doDiscard,
                            Enabled = { Value = false },
                            Name = "Discard"
                        },
                        skipButton = new RoundedButton
                        {
                            Text = "Skip",
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(100, 40),
                            Action = () =>
                            {
                                // TODO
                            }
                        },
                    }
                },
            ];

            const float spacing = -20;
            float totalWidth = cardFlow.Children.Skip(1).Sum(card => card.Width + spacing) - spacing;

            float x = -totalWidth / 2;

            foreach (var card in cardFlow)
            {
                card.X = x;
                card.Y = MathF.Pow(MathF.Abs(x / (totalWidth / 2)), 2) * 20;
                card.Rotation = x * 0.03f;
                x += card.Width + spacing;
            }
        }

        private void cardClicked(Card target)
        {
            if (didDiscard)
                return;

            target.Selected = !target.Selected;

            discardButton.Enabled.Value = cardFlow.Any(it => it.Selected);
        }

        private bool didDiscard;

        private void doDiscard()
        {
            didDiscard = true;

            var cards = cardFlow.Where(it => it.Selected).ToList();

            const float spacing = 20;
            float totalWidth = cards.Skip(1).Sum(it => it.Width + spacing) - spacing;
            float x = -totalWidth / 2;

            foreach (var card in cards)
            {
                card.MoveTo(new Vector2(x, -200), 400, Easing.OutExpo)
                    .RotateTo(0, 400, Easing.OutExpo);
                x += card.Width + spacing;
            }
        }

        public partial class Card : CompositeDrawable
        {
            public readonly MultiplayerPlaylistItem Item;

            private readonly Drawable content;

            public Card(MultiplayerPlaylistItem item)
            {
                Item = item;
                Size = new Vector2(150, 250);

                InternalChild = content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 10,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(0.1f),
                    },
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Color4.Gray
                        },
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.Both,
                            Children =
                            [
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = $"Playlist Item Id {item.ID}",
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = $"Beatmap Id {item.BeatmapID}",
                                }
                            ]
                        }
                    ]
                };
            }

            public required Action<Card> Clicked;

            private bool selected;

            public bool Selected
            {
                get => selected;
                set
                {
                    selected = value;
                    content.MoveToY(value ? -100 : 0, 400, Easing.OutElasticQuarter);
                }
            }

            protected override bool OnClick(ClickEvent e)
            {
                Clicked(this);
                return true;
            }
        }
    }
}
