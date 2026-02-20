// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.RankedPlay;
using osuTK;
using osuTK.Input;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    /// <summary>
    /// Card hand representing the player's current hand, intended to be placed at the bottom edge of the screen.
    /// This version of the card hand reacts to player inputs like hovering a card.
    /// </summary>
    [Cached]
    public partial class PlayerCardHand : CardHand
    {
        /// <summary>
        /// Fired if any card is selected or deselected
        /// </summary>
        public event Action? SelectionChanged;

        /// <summary>
        /// Fired if a card's <see cref="CardHand.HandCard.State"/> has changed
        /// </summary>
        public event Action? StateChanged;

        private CardSelectionMode selectionMode;

        /// <summary>
        /// Current selection mode.
        /// </summary>
        /// <remarks>
        /// <see cref="CardSelectionMode.Disabled"/> will disable some of the card's mouse interactions.
        /// </remarks>
        public CardSelectionMode SelectionMode
        {
            get => selectionMode;
            set
            {
                selectionMode = value;
                allowSelection.Value = value != CardSelectionMode.Disabled;

                if (value == CardSelectionMode.Disabled)
                {
                    foreach (var card in Cards)
                        card.Selected = false;
                }
            }
        }

        private Action? playCardAction;

        /// <summary>
        /// When set to non-null, displays a "Play" button on the selected card that invokes this action.
        /// </summary>
        public Action? PlayCardAction
        {
            get => playCardAction;
            set
            {
                playCardAction = value;

                foreach (var card in Cards.OfType<PlayerHandCard>())
                    card.PlayAction = value;
            }
        }

        private IEnumerable<PlayerHandCard> selection => Cards.OfType<PlayerHandCard>().Where(it => it.Selected);

        /// <summary>
        /// Currently selected cards.
        /// </summary>
        public IEnumerable<RankedPlayCardWithPlaylistItem> Selection => selection.Select(it => it.Card.Item);

        private readonly BindableBool allowSelection = new BindableBool();

        private const int select_samples = 1;
        private const int deselect_samples = 2;

        private Sample?[]? cardSelectSamples;
        private Sample?[]? cardDeselectSamples;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            cardSelectSamples = new Sample?[select_samples];
            for (int i = 0; i < select_samples; i++)
                cardSelectSamples[i] = audio.Samples.Get(@$"Multiplayer/Matchmaking/Ranked/card-select-{i + 1}");

            cardDeselectSamples = new Sample?[deselect_samples];
            for (int i = 0; i < deselect_samples; i++)
                cardDeselectSamples[i] = audio.Samples.Get(@$"Multiplayer/Matchmaking/Ranked/card-deselect-{i + 1}");
        }

        protected override HandCard CreateHandCard(RankedPlayCard card) => new PlayerHandCard(card)
        {
            Clicked = cardClicked,
            AllowSelection = allowSelection.GetBoundCopy(),
            PlayAction = PlayCardAction,
        };

        private void cardClicked(PlayerHandCard card)
        {
            if (selectionMode == CardSelectionMode.Disabled)
                return;

            try
            {
                if (selectionMode == CardSelectionMode.Single)
                {
                    // only play feedback SFX if the selected card has changed
                    if (!card.Selected)
                        SamplePlaybackHelper.PlayWithRandomPitch(cardSelectSamples);

                    foreach (var c in Cards)
                    {
                        ((PlayerHandCard)c).Selected = c == card;
                    }

                    return;
                }

                card.Selected = !card.Selected;

                SamplePlaybackHelper.PlayWithRandomPitch(card.Selected ? cardSelectSamples : cardDeselectSamples);
            }
            finally
            {
                SelectionChanged?.Invoke();
            }
        }

        protected override void OnCardStateChanged(HandCard card, RankedPlayCardState state)
        {
            StateChanged?.Invoke();

            base.OnCardStateChanged(card, state);
        }

        public Dictionary<Guid, RankedPlayCardState> State => Cards.Select(static card => new KeyValuePair<Guid, RankedPlayCardState>(card.Item.Card.ID, card.State)).ToDictionary();

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
            private PlayerCardHand cardHand { get; set; } = null!;

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
                            Child = playButton = new ShearedButton(width: 90f, height: 30f)
                            {
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
                if (!AllowSelection.Value || cardHand.selectionMode != CardSelectionMode.Single)
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

                    swipeRevealContainer.Y = remap(swipeProgress, (0, 0.8f), (10, 2));

                    swipeArrows.Y = remap(swipeProgress, from: (0, 0.6f), to: (-5, -35));
                    swipeRevealText.Y = remap(swipeProgress, from: (0, 0.6f), to: (0, -9));

                    swipeArrows.Height = remap(swipeProgress, from: (0, 0.55f), to: (10, 20));

                    swipeRevealContainer.Alpha = (float)Interpolation.DampContinuously(swipeRevealContainer.Alpha, swipeProgress > swipe_threshold ? 1 : 0, 30, Time.Elapsed);
                }
            }

            /// <summary>
            /// Remaps a value from one range to another while clamping the input value to the input range.
            /// </summary>
            private static float remap(float value, (float lower, float upper) from, (float lower, float upper) to)
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
