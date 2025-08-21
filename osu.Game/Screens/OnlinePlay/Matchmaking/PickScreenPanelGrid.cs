// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking
{
    public partial class PickScreenPanelGrid : CompositeDrawable
    {
        private const double arrange_delay = 200;
        private const double arrange_duration = 1000;
        private const double roll_duration = 4000;

        public event Action<MultiplayerPlaylistItem>? ItemSelected;

        private readonly Dictionary<long, BeatmapSelectionPanel> items = new Dictionary<long, BeatmapSelectionPanel>();
        private readonly Dictionary<int, long> userSelection = new Dictionary<int, long>();

        private readonly PanelContainer panelContainer;
        private readonly Container<BeatmapSelectionPanel> finalPanelContainer;
        private readonly OsuScrollContainer scroll;
        private readonly Box background;

        private bool allowSelection = true;

        public PickScreenPanelGrid()
        {
            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Depth = float.MaxValue,
                    Colour = Color4.Black,
                },
                scroll = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = panelContainer = new PanelContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(20),
                        Spacing = new Vector2(20)
                    },
                },
                finalPanelContainer = new Container<BeatmapSelectionPanel>
                {
                    RelativeSizeAxes = Axes.Both,
                },
            };
        }

        public void AddItem(MultiplayerPlaylistItem item)
        {
            var panel = items[item.ID] = new BeatmapSelectionPanel(item)
            {
                Size = new Vector2(300, 70),
                AllowSelection = allowSelection,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.LightSlateGray,
                }
            };

            panel.Clicked += () => ItemSelected?.Invoke(item);

            panelContainer.Add(panel);
            panelContainer.SetLayoutPosition(panel, (float)item.StarRating);
        }

        public void RemoveItem(long id)
        {
            if (!items.Remove(id, out var panel))
                return;

            panelContainer.Remove(panel, true);
        }

        public void SetUserSelection(APIUser user, long itemId, bool isOwnUser)
        {
            long? oldItemId = userSelection.TryGetValue(user.Id, out long id) ? id : null;

            if (oldItemId == itemId)
                return;

            if (oldItemId != null && items.TryGetValue(oldItemId.Value, out var oldPanel))
                oldPanel.RemoveUser(user.Id);

            if (items.TryGetValue(itemId, out var newPanel))
                newPanel.AddUser(user, isOwnUser);

            userSelection[user.Id] = itemId;
        }

        public void PresentFinalBeatmap(long[] candidateItems, long finalItem)
        {
            Debug.Assert(items.ContainsKey(finalItem));
            Debug.Assert(candidateItems.All(id => items.ContainsKey(id)));

            scroll.ScrollbarVisible = false;
            panelContainer.LayoutDisabled = true;
            allowSelection = false;

            var rng = new Random();

            foreach (var panel in panelContainer.Children.ToArray())
            {
                panel.AllowSelection = false;

                if (!candidateItems.Contains(panel.Item.ID))
                {
                    panel.PopOutAndExpire(rng.NextDouble() * 500);
                    continue;
                }

                var position = panel.ScreenSpaceDrawQuad.Centre;

                panelContainer.Remove(panel, false);

                panel.Anchor = panel.Origin = Anchor.Centre;
                panel.Position = finalPanelContainer.ToLocalSpace(position) - finalPanelContainer.ChildSize / 2;

                finalPanelContainer.Add(panel);
            }

            this.Delay(arrange_delay)
                .Schedule(arrangeItemsForRollAnimation)
                .Delay(arrange_duration)
                .Schedule(() => playRollAnimation(finalItem))
                .Delay(roll_duration + 1000)
                .Schedule(() => displayFinalBeatmap(finalItem));
        }

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        private void arrangeItemsForRollAnimation()
        {
            const float spacing = 20f;
            const double stagger_duration = 20;

            int numPanelsPerRow = Math.Min((int)((finalPanelContainer.ChildSize.X + spacing) / (BeatmapSelectionPanel.SIZE.X + spacing)), 3);
            int numRows = (finalPanelContainer.Children.Count + numPanelsPerRow - 1) / numPanelsPerRow;

            float totalHeight = (numRows * (BeatmapSelectionPanel.SIZE.Y + spacing)) - spacing;

            var position = new Vector2(calculateNextRowStart(0), -totalHeight / 2);

            int lastRowPanelCount = finalPanelContainer.Children.Count % numPanelsPerRow;
            bool offsetOuterPanels = lastRowPanelCount == 1;

            double delay = 0;

            for (int i = 0; i < finalPanelContainer.Children.Count; i++)
            {
                var panel = finalPanelContainer.Children[i];
                int rowIndex = i / numPanelsPerRow;
                int positionInRow = i % numPanelsPerRow;

                if (position.X + BeatmapSelectionPanel.SIZE.X > finalPanelContainer.ChildSize.X / 2)
                {
                    position.X = calculateNextRowStart(i);
                    position.Y += BeatmapSelectionPanel.SIZE.Y + spacing;
                }

                var panelPosition = position + BeatmapSelectionPanel.SIZE / 2;

                if (offsetOuterPanels && rowIndex < numRows - 1 && positionInRow is 0 or 2)
                    panelPosition.Y += (BeatmapSelectionPanel.SIZE.Y + spacing) / 2;

                panel.Delay(delay)
                     .MoveTo(panelPosition, arrange_duration, Easing.InOutQuint);

                position.X += BeatmapSelectionPanel.SIZE.X + spacing;

                delay += stagger_duration;
            }

            float calculateNextRowStart(int currentIndex)
            {
                int remaining = Math.Min(finalPanelContainer.Children.Count - currentIndex, numPanelsPerRow);
                float nextRowWidth = ((BeatmapSelectionPanel.SIZE.X + spacing) * remaining - spacing);

                return -nextRowWidth / 2;
            }
        }

        private void playRollAnimation(long finalItem)
        {
            const int minimum_steps = 15;

            int finalItemIndex = finalPanelContainer.Children
                                                    .Select(it => it.Item.ID)
                                                    .ToImmutableList()
                                                    .IndexOf(finalItem);

            Debug.Assert(finalItemIndex >= 0);

            int numSteps = minimum_steps;
            while ((numSteps - 1) % finalPanelContainer.Count != finalItemIndex)
                numSteps++;

            BeatmapSelectionPanel? lastPanel = null;

            for (int i = 0; i < numSteps; i++)
            {
                float progress = ((float)i) / (numSteps - 1);

                double delay = Math.Pow(progress, 2.5) * roll_duration;

                int index = i;

                Scheduler.AddDelayed(() =>
                {
                    var panel = finalPanelContainer.Children[index % finalPanelContainer.Children.Count];

                    lastPanel?.HideBorder();
                    panel.ShowBorder();

                    lastPanel = panel;
                }, delay);
            }
        }

        private void displayFinalBeatmap(long finalItem)
        {
            Debug.Assert(finalPanelContainer.Children.Any(it => it.Item.ID == finalItem));

            foreach (var panel in finalPanelContainer)
            {
                if (panel.Item.ID != finalItem)
                {
                    panel.FadeOut(200);
                    panel.PopOutAndExpire(easing: Easing.InOutQuad);
                    continue;
                }

                var drawQuad = panel.ScreenSpaceDrawQuad.AABBFloat;

                var position = new Vector2(drawQuad.Right, drawQuad.Centre.Y);

                panel.Origin = Anchor.CentreRight;

                panel.Position = finalPanelContainer.ToLocalSpace(position) - finalPanelContainer.ChildSize / 2;

                panel.MoveTo(new Vector2(-20, 0), 1000, Easing.OutPow10)
                     .ScaleTo(1.5f, 1000, Easing.OutExpo);

                var ornament = new SelectionOrnament(panel.Item)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.CentreLeft,
                    Position = ToLocalSpace(new Vector2(drawQuad.Left, drawQuad.Centre.Y)) - finalPanelContainer.ChildSize / 2,
                    Alpha = 0,
                    Depth = 1,
                };

                AddInternal(ornament);

                ornament.MoveTo(new Vector2(20, 0), 1000, Easing.OutPow10)
                        .FadeIn(400);

                beatmapLookupCache.GetBeatmapAsync(panel.Item.BeatmapID).ContinueWith(b => Schedule(() =>
                {
                    var beatmap = b.GetResultSafely();

                    Box box1, box2;
                    Drawable cover;
                    Container container;

                    AddInternal(new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Depth = 2,
                        Children = new Drawable[]
                        {
                            cover = new UpdateableOnlineBeatmapSetCover(timeBeforeLoad: 0)
                            {
                                RelativeSizeAxes = Axes.Both,
                                OnlineInfo = beatmap?.BeatmapSet,
                                Alpha = 0.25f,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            },
                            container = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Position = panel.Position,
                                Children = new Drawable[]
                                {
                                    box1 = new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        RelativePositionAxes = Axes.X,
                                        Size = new Vector2(3),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.CentreRight,
                                        Colour = Color4.Black,
                                    },
                                    box2 = new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        RelativePositionAxes = Axes.X,
                                        Size = new Vector2(3),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.CentreLeft,
                                        Colour = Color4.Black,
                                    },
                                }
                            }
                        }
                    });

                    box1.MoveToX(-0.46f, 2000, Easing.OutPow10);
                    box2.MoveToX(0.46f, 2000, Easing.OutPow10);
                    cover.FadeTo(0).FadeTo(0.25f, 500)
                         .ScaleTo(2f)
                         .ScaleTo(1f, 3000, Easing.OutPow10);

                    background.FadeColour(Color4.Black, 1000);

                    container
                        .MoveTo(Vector2.Zero, 1000, Easing.OutPow10)
                        .RotateTo(-10)
                        .RotateTo(15, 3000, Easing.OutPow10);
                }));
            }
        }

        private partial class PanelContainer : FillFlowContainer<BeatmapSelectionPanel>
        {
            public bool LayoutDisabled;

            protected override IEnumerable<Vector2> ComputeLayoutPositions()
            {
                if (LayoutDisabled)
                    return FlowingChildren.Select(c => c.Position);

                return base.ComputeLayoutPositions();
            }
        }

        private partial class SelectionOrnament : CompositeDrawable
        {
            private readonly MultiplayerPlaylistItem item;

            [Resolved]
            private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

            public SelectionOrnament(MultiplayerPlaylistItem item)
            {
                this.item = item;
            }

            [BackgroundDependencyLoader]
            private void load(BeatmapLookupCache beatmapLookupCache)
            {
                FillFlowContainer content;

                AutoSizeAxes = Axes.Both;

                InternalChild = content = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = "Selected Beatmap",
                            Font = OsuFont.TorusAlternate.With(size: 50),
                        },
                    }
                };

                beatmapLookupCache.GetBeatmapAsync(item.BeatmapID).ContinueWith(b => Schedule(() =>
                {
                    APIBeatmap beatmap = b.GetResultSafely()!;

                    AutoSizeDuration = 400;
                    AutoSizeEasing = Easing.OutExpo;

                    OsuSpriteText title, artist, extras;

                    content.AddRange(new Drawable[]
                    {
                        title = new OsuSpriteText
                        {
                            Text = beatmap.BeatmapSet!.Title,
                            Font = OsuFont.Style.Heading1,
                            Margin = new MarginPadding { Top = 5 },
                        },
                        artist = new OsuSpriteText
                        {
                            Text = $"by {beatmap.BeatmapSet!.Artist}",
                            Font = OsuFont.Style.Heading2,
                        },
                        extras = new OsuSpriteText
                        {
                            Text = "TODO: add more stuff here",
                            Font = OsuFont.Style.Heading2,
                        },
                    });

                    title.FadeOut()
                         .Delay(1000)
                         .FadeIn();

                    artist.FadeOut()
                          .Delay(1080)
                          .FadeIn();

                    extras.FadeOut()
                          .Delay(1160)
                          .FadeIn();
                }));
            }
        }
    }
}
