// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.RankedPlay;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Card;
using osuTK;
using osuTK.Input;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Hand
{
    public partial class PlayerHandOfCards
    {
        public partial class PlayerHandCard : HandCard
        {
            private const float swipe_threshold = 0.2f;

            private float swipeProgress;

            private Vector2 targetDragPosition;

            private readonly Vector2Spring dragPositionSpring = new Vector2Spring
            {
                NaturalFrequency = 4f,
                Damping = 0.7f,
                Response = 0.5f,
            };

            [Resolved]
            private PlayerHandOfCards handOfCards { get; set; } = null!;

            private Action? playAction;

            public Action? PlayAction
            {
                set
                {
                    playAction = value;
                    playButton.Action = value;
                    updatePlayButtonVisibility();
                }
            }

            public required Action<PlayerHandCard> Clicked;

            public required IBindable<bool> AllowSelection;

            private readonly Drawable cardInputArea;
            private readonly Drawable fullInputArea;

            private readonly ShearedButton playButton;

            private readonly Container swipeRevealContainer;
            private readonly Container swipeArrows;
            private readonly OsuSpriteText swipeRevealText;

            public PlayerHandCard(RankedPlayCard card)
                : base(card)
            {
                AddRangeInternal(new Drawable[]
                {
                    swipeRevealContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = RankedPlayCard.CORNER_RADIUS,
                        Alpha = 0,
                        Depth = 1,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(0.9f),
                        Children =
                        [
                            swipeRevealText = new OsuSpriteText
                            {
                                Text = "Release to play",
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre,
                                Font = OsuFont.GetFont(weight: FontWeight.Medium),
                                Shadow = false,
                                Colour = Colour4.FromHex("BFECFF"),
                                Blending = BlendingParameters.Additive,
                            },
                            swipeArrows = new Container
                            {
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre,
                                Colour = Colour4.FromHex("BFECFF"),
                                Blending = BlendingParameters.Additive,
                                Children =
                                [
                                    new SpriteIcon
                                    {
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.BottomCentre,
                                        Icon = FontAwesome.Solid.ChevronUp,
                                        Size = new Vector2(20),
                                    },
                                    new SpriteIcon
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.Centre,
                                        Icon = FontAwesome.Solid.ChevronUp,
                                        Size = new Vector2(20),
                                    }
                                ]
                            },
                        ]
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(-10),
                        Child = cardInputArea = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = -40 },
                        Child = fullInputArea = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = playButton = new ShearedButton
                            {
                                Size = new Vector2(90f, 30f),
                                Name = "Play Button",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "Play",
                                TextSize = 14,
                                LighterColour = Colour4.FromHex("87D8FA"),
                                DarkerColour = Colour4.FromHex("72D5FF")
                            }
                        }
                    }
                });
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                AddInternal(new HoverSounds());
            }

            protected override void OnStateChanged(ValueChangedEvent<RankedPlayCardState> state)
            {
                base.OnStateChanged(state);
                updatePlayButtonVisibility();
            }

            private void updatePlayButtonVisibility()
            {
                bool visible = playButton.Action != null && Selected && !IsDragged;

                playButton.Alpha = visible ? 1 : 0;
            }

            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
            {
                if (playButton.Alpha > 0)
                    return fullInputArea.ReceivePositionalInputAt(screenSpacePos);

                // input events are handled for an area that's slightly larger than the actual card so the cursor always hovers a card when moving over a gap between two cards
                return cardInputArea.ReceivePositionalInputAt(screenSpacePos);
            }

            protected override bool OnHover(HoverEvent e)
            {
                CardHovered = true;

                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                if (IsDragged)
                    return;

                CardHovered = false;
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                if (e.Button == MouseButton.Left && AllowSelection.Value)
                {
                    CardPressed = true;

                    return true;
                }

                return false;
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                if (e.Button == MouseButton.Left)
                    CardPressed = false;
            }

            protected override bool OnClick(ClickEvent e)
            {
                if (!AllowSelection.Value)
                    return false;

                Clicked(this);

                return true;
            }

            protected override bool OnDragStart(DragStartEvent e)
            {
                if (!AllowSelection.Value || handOfCards.SelectionMode != HandSelectionMode.Single)
                    return false;

                Schedule(updatePlayButtonVisibility);

                dragPositionSpring.Current = Vector2.Zero;
                dragPositionSpring.PreviousTarget = Vector2.Zero;

                return true;
            }

            protected override void OnDrag(DragEvent e)
            {
                base.OnDrag(e);

                targetDragPosition = e.MousePosition - e.MouseDownPosition;
            }

            protected override void Update()
            {
                base.Update();

                if (IsDragged)
                {
                    var dragPosition = dragPositionSpring.Update(Time.Elapsed, targetDragPosition);

                    swipeProgress = MathF.Pow(float.Max(-dragPosition.Y / 600f, 0), 0.8f);

                    Card.Y = swipeProgress * -160;
                    Card.X = dragPosition.X * 0.1f;
                    Card.Rotation = Card.X * 0.1f;

                    swipeRevealContainer.Y = remapClamped(swipeProgress, (0, 0.8f), (10, 2));

                    swipeArrows.Y = remapClamped(swipeProgress, from: (0, 0.6f), to: (-5, -35));
                    swipeRevealText.Y = remapClamped(swipeProgress, from: (0, 0.6f), to: (0, -9));

                    swipeArrows.Height = remapClamped(swipeProgress, from: (0, 0.55f), to: (10, 20));

                    swipeRevealContainer.Alpha = (float)Interpolation.DampContinuously(swipeRevealContainer.Alpha, swipeProgress > swipe_threshold ? 1 : 0, 30, Time.Elapsed);
                }
            }

            /// <summary>
            /// Remaps a value from one range to another while clamping the input value to the input range.
            /// </summary>
            private static float remapClamped(float value, (float lower, float upper) from, (float lower, float upper) to)
            {
                Debug.Assert(from.upper != from.lower);

                float progress = float.Clamp((value - from.lower) / (from.upper - from.lower), 0, 1);

                return progress * (to.upper - to.lower) + to.lower;
            }

            protected override void OnDragEnd(DragEndEvent e)
            {
                base.OnDragEnd(e);

                updatePlayButtonVisibility();

                if (swipeProgress > swipe_threshold && playAction is { } action)
                {
                    Clicked(this);
                    action();
                    return;
                }

                Card.MoveToY(0, 400, Easing.OutElasticHalf);
                Card.MoveToX(0, 400, Easing.OutElasticHalf);
                Card.RotateTo(0, 400, Easing.OutElasticHalf);

                swipeRevealContainer.MoveToY(0, 400, Easing.OutElasticHalf)
                                    .FadeOut(200);

                if (!IsHovered)
                    CardHovered = false;
            }
        }
    }
}
