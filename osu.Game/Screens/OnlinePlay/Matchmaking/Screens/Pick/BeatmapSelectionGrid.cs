// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit.HighPerformance;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Overlays;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Pick
{
    public partial class BeatmapSelectionGrid : CompositeDrawable
    {
        public const double ARRANGE_DELAY = 200;

        private const double hide_duration = 800;
        private const double arrange_duration = 1000;
        private const double roll_duration = 4000;
        private const double present_beatmap_delay = 1200;
        private const float panel_spacing = 20;

        public event Action<MultiplayerPlaylistItem>? ItemSelected;

        [Resolved]
        private IAPIProvider api { get; set; } = null!;

        [Resolved]
        private OverlayColourProvider? colourProvider { get; set; }

        private readonly Dictionary<long, BeatmapSelectionPanel> panelLookup = new Dictionary<long, BeatmapSelectionPanel>();

        private readonly PanelGridContainer panelGridContainer;
        private readonly Container<BeatmapSelectionPanel> rollContainer;
        private readonly OsuScrollContainer scroll;

        private bool allowSelection = true;

        public BeatmapSelectionGrid()
        {
            InternalChildren = new Drawable[]
            {
                scroll = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarVisible = false,
                    Child = panelGridContainer = new PanelGridContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(20),
                        Spacing = new Vector2(panel_spacing)
                    },
                },
                rollContainer = new Container<BeatmapSelectionPanel>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                },
            };
        }

        private Sample? tickSample;
        private Sample? presentSample;
        private Sample? windupSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            tickSample = audio.Samples.Get("UI/notch-tick");
            windupSample = audio.Samples.Get("Results/swoosh-up");
            presentSample = audio.Samples.Get("SongSelect/confirm-selection");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            const double enter_duration = 500;

            // the scroll container has a 1 frame delay until it receives the correct height for the scrollable area which leads to the scrollbar resizing awkwardly
            // if we wait until the panels have entered we get to avoid having to see that and the scrollbar it will appear synchronized with the rest of the content as a bonus
            Scheduler.AddDelayed(() => scroll.ScrollbarVisible = true, enter_duration);

            SchedulerAfterChildren.Add(() =>
            {
                foreach (var panel in panelGridContainer)
                {
                    double delay = panel.Y / 3;

                    panel.FadeInAndEnterFromBelow(duration: enter_duration, delay: delay);
                }
            });
        }

        public void AddItem(MultiplayerPlaylistItem item)
        {
            var panel = panelLookup[item.ID] = new BeatmapSelectionPanel(item)
            {
                Size = new Vector2(300, 70),
                AllowSelection = allowSelection,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Action = ItemSelected,
            };

            panelGridContainer.Add(panel);
            panelGridContainer.SetLayoutPosition(panel, (float)item.StarRating);
        }

        public void RemoveItem(long id)
        {
            if (!panelLookup.Remove(id, out var panel))
                return;

            panelGridContainer.Remove(panel, true);
        }

        public void SetUserSelection(APIUser user, long itemId, bool selected)
        {
            if (!panelLookup.TryGetValue(itemId, out var panel))
                return;

            if (selected)
                panel.AddUser(user, user.Equals(api.LocalUser.Value));
            else
                panel.RemoveUser(user);
        }

        public void RollAndDisplayFinalBeatmap(long[] candidateItemIds, long finalItemId)
        {
            Debug.Assert(candidateItemIds.Length >= 1);
            Debug.Assert(candidateItemIds.Contains(finalItemId));
            Debug.Assert(panelLookup.ContainsKey(finalItemId));
            Debug.Assert(candidateItemIds.All(id => panelLookup.ContainsKey(id)));

            allowSelection = false;

            TransferCandidatePanelsToRollContainer(candidateItemIds);

            if (candidateItemIds.Length == 1)
            {
                this.Delay(ARRANGE_DELAY)
                    .Schedule(() => ArrangeItemsForRollAnimation())
                    .Delay(arrange_duration)
                    .Schedule(() => PresentUnanimouslyChosenBeatmap(finalItemId));
            }
            else
            {
                this.Delay(ARRANGE_DELAY)
                    .Schedule(() => ArrangeItemsForRollAnimation())
                    .Delay(arrange_duration)
                    .Schedule(() => PlayRollAnimation(finalItemId, roll_duration))
                    .Delay(roll_duration)
                    .Schedule(() => PresentRolledBeatmap(finalItemId));
            }
        }

        internal void TransferCandidatePanelsToRollContainer(long[] candidateItemIds, double duration = hide_duration)
        {
            scroll.ScrollbarVisible = false;
            panelGridContainer.LayoutDisabled = true;

            var rng = new Random();

            var remainingPanels = new List<BeatmapSelectionPanel>();

            foreach (var panel in panelGridContainer.Children.ToArray())
            {
                panel.AllowSelection = false;

                if (!candidateItemIds.Contains(panel.Item.ID))
                {
                    panel.PopOutAndExpire(duration: duration / 2, delay: rng.NextDouble() * duration / 2);
                    continue;
                }

                remainingPanels.Add(panel);
            }

            rng.Shuffle(remainingPanels.AsSpan());

            foreach (var panel in remainingPanels)
            {
                var position = panel.ScreenSpaceDrawQuad.Centre;

                panelGridContainer.Remove(panel, false);

                panel.Anchor = panel.Origin = Anchor.Centre;
                panel.Position = rollContainer.ToLocalSpace(position) - rollContainer.ChildSize / 2;

                rollContainer.Add(panel);
            }
        }

        internal void ArrangeItemsForRollAnimation(double duration = arrange_duration, double stagger = 30)
        {
            var positions = calculateLayoutPositionsForRollAnimation(rollContainer.Children.Count);

            Debug.Assert(positions.Length == rollContainer.Children.Count);

            for (int i = 0; i < positions.Length; i++)
            {
                var panel = rollContainer.Children[i];

                var position = positions[i] * (BeatmapPanel.SIZE + new Vector2(panel_spacing));

                panel.MoveTo(position, duration + stagger * i, new SplitEasingFunction(Easing.InCubic, Easing.OutExpo, 0.3f));
            }
        }

        private static Vector2[] calculateLayoutPositionsForRollAnimation(int panelCount)
        {
            if (panelCount == 1)
                return new[] { Vector2.Zero };

            // goal is to get the positions arranged in clockwise order, with the top-left position being the first one
            // to keep things simple the positions are first inserted in the order: right row, optional bottom center panel, left row backwards
            // then the positions get shifted by 1 to move the top-left position into the first spot

            bool hasCenterPanel = panelCount % 2 == 1;
            int rowCount = (panelCount + 1) / 2;
            int outerRowCount = hasCenterPanel ? rowCount - 1 : rowCount;

            float yOffset = -(rowCount - 1f) / 2;

            var positions = new Vector2[panelCount];

            for (int row = 0; row < outerRowCount; row++)
            {
                positions[row] = new Vector2(0.5f, row + yOffset);
            }

            if (hasCenterPanel)
            {
                int centerIndex = panelCount / 2;

                positions[centerIndex] = new Vector2(0, outerRowCount + yOffset);
            }

            for (int row = 0; row < outerRowCount; row++)
            {
                int index = positions.Length - 1 - row;

                positions[index] = new Vector2(-0.5f, row + yOffset);
            }

            return positions.TakeLast(1).Concat(positions.SkipLast(1)).ToArray();
        }

        internal void PlayRollAnimation(long finalItem, double duration = roll_duration)
        {
            const int minimum_steps = 20;

            int finalItemIndex = rollContainer.Children
                                              .Select(it => it.Item.ID)
                                              .ToImmutableList()
                                              .IndexOf(finalItem);

            Debug.Assert(finalItemIndex >= 0);

            int numSteps = minimum_steps;
            while ((numSteps - 1) % rollContainer.Children.Count != finalItemIndex)
                numSteps++;

            BeatmapSelectionPanel? lastPanel = null;

            for (int i = 0; i < numSteps; i++)
            {
                float progress = ((float)i) / (numSteps - 1);

                double delay = Math.Pow(progress, 2.5) * duration;
                double frequency = 1.1 - progress * 0.1;
                var panel = rollContainer.Children[i % rollContainer.Children.Count];

                Scheduler.AddDelayed(() =>
                {
                    lastPanel?.HideBorder();
                    panel.ShowBorder();

                    var channel = tickSample?.Play();

                    if (channel != null)
                        channel.Frequency.Value = frequency;

                    lastPanel = panel;
                }, delay);
            }
        }

        internal void PresentRolledBeatmap(long finalItem)
        {
            Debug.Assert(rollContainer.Children.Any(it => it.Item.ID == finalItem));

            foreach (var panel in rollContainer.Children)
            {
                if (panel.Item.ID != finalItem)
                {
                    panel.PopOutAndExpire(
                        delay: Random.Shared.NextDouble() * 600,
                        duration: 700,
                        easing: Easing.InQuad
                    );
                    continue;
                }

                // if we changed child depth without scheduling we'd change the order of the panels while iterating
                Schedule(() => rollContainer.ChangeChildDepth(panel, float.MinValue));

                windupSample?.Play();

                double windupDuration = windupSample?.Length ?? 1000;

                panel.ShowBorder();
                panel.MoveTo(Vector2.Zero, windupDuration, Easing.InCubic)
                     .ScaleTo(1.75f, windupDuration, Easing.InExpo);

                var flash = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = 6,
                    Alpha = 0,
                    Blending = BlendingParameters.Additive,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Glow,
                        Radius = 100,
                        Colour = (colourProvider?.Light2 ?? Color4.White).Opacity(0),
                        Roundness = 50
                    },
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                };

                panel.Add(flash);
                panel.Add(new WindupAnimation
                {
                    RelativeSizeAxes = Axes.Both,
                    Duration = windupDuration,
                    Depth = float.MaxValue,
                    Blending = BlendingParameters.Additive,
                });

                flash.FadeIn(windupDuration, Easing.InCubic)
                     .FadeEdgeEffectTo(0.25f, windupDuration, Easing.In)
                     .Then()
                     .ScaleTo(1.5f, 400, Easing.OutCubic)
                     .FadeOut();

                Scheduler.AddDelayed(() =>
                {
                    presentSample?.Play();

                    panel.HideBorder();
                    panel.PresentCover();
                    panel.MoveToX(-BeatmapPanel.SIZE.X * 1.4f / 2 - 20, 1000, Easing.OutExpo)
                         .ScaleTo(1.4f, 1000, Easing.OutElasticHalf);

                    var ripple = new Circle
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fill,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0,
                        Blending = BlendingParameters.Additive,
                    };

                    AddInternal(ripple);

                    // Final scale must be kept above 1.41x (sqrt 2) to cover the entire grid
                    ripple
                        .FadeTo(0.5f)
                        .FadeOut(1000, Easing.Out)
                        .ScaleTo(0)
                        .ScaleTo(1.5f, 1500, Easing.OutExpo);

                    var text = new OsuSpriteText
                    {
                        Text = "Selected beatmap",
                        Font = OsuFont.TorusAlternate.With(size: 50),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.CentreLeft,
                        Depth = float.MaxValue,
                    };

                    AddInternal(text);

                    text
                        .MoveToX(-250)
                        .MoveToX(20, 1000, Easing.OutExpo)
                        .FadeInFromZero(200);
                }, windupSample?.Length ?? 1000);
            }
        }

        internal void PresentUnanimouslyChosenBeatmap(long finalItem)
        {
            // TODO: display special animation in this case

            PresentRolledBeatmap(finalItem);
        }

        private partial class PanelGridContainer : FillFlowContainer<BeatmapSelectionPanel>
        {
            public bool LayoutDisabled;

            protected override IEnumerable<Vector2> ComputeLayoutPositions()
            {
                if (LayoutDisabled)
                    return FlowingChildren.Select(c => c.Position);

                return base.ComputeLayoutPositions();
            }
        }

        private readonly struct SplitEasingFunction(DefaultEasingFunction easeIn, DefaultEasingFunction easeOut, float ratio) : IEasingFunction
        {
            public SplitEasingFunction(Easing easeIn, Easing easeOut, float ratio = 0.5f)
                : this(new DefaultEasingFunction(easeIn), new DefaultEasingFunction(easeOut), ratio)
            {
            }

            public double ApplyEasing(double time)
            {
                if (time < ratio)
                    return easeIn.ApplyEasing(time / ratio) * ratio;

                return double.Lerp(ratio, 1, easeOut.ApplyEasing((time - ratio) / (1 - ratio)));
            }
        }
    }
}
