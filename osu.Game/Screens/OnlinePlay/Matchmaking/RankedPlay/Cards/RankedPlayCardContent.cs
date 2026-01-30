// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Localisation;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent : CompositeDrawable, IHasContextMenu
    {
        public readonly APIBeatmap Beatmap;

        private CardColours colours = null!;
        private PreviewTrack? preview;
        private PreviewVisualization previewVisualization = null!;

        [Resolved]
        private CardDetailsOverlayContainer? cardDetailsOverlay { get; set; }

        [Resolved]
        private PreviewTrackManager previewTrackManager { get; set; } = null!;

        public RankedPlayCardContent(APIBeatmap beatmap)
        {
            Size = RankedPlayCard.SIZE;

            Beatmap = beatmap;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                previewVisualization = new PreviewVisualization(Beatmap),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = RankedPlayCard.CORNER_RADIUS,
                    Children =
                    [
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colours.Background,
                        },
                        new Container
                        {
                            Name = "Top Area",
                            RelativeSizeAxes = Axes.Both,
                            FillMode = FillMode.Fit,
                            Children =
                            [
                                new CardCover(Beatmap)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new CardMetadata(Beatmap)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                },
                                new DifficultyNameBadge(Beatmap)
                                {
                                    Width = 100,
                                    AutoSizeAxes = Axes.Y,

                                    // this container partially overlaps with the bottom area
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.Centre,
                                }
                            ],
                        },
                        new Container
                        {
                            Name = "Bottom Area",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = RankedPlayCard.SIZE.X + 6 },
                            Children =
                            [
                                new AttributeListing(Beatmap)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                }
                            ]
                        },
                    ]
                },
                new CardBorder()
            ];

            LoadComponentAsync(preview = previewTrackManager.Get(Beatmap.BeatmapSet!), preview =>
            {
                AddInternal(preview);

                preview.Started += previewVisualization.PreviewStarted;
                preview.Stopped += previewVisualization.PreviewStopped;

                if (IsHovered)
                    preview.Start();
            });
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(colours = new CardColours(Beatmap, dependencies.Get<OsuColour>()));

            return dependencies;
        }

        public override bool HandlePositionalInput => true;

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            if (IsHovered)
                cardDetailsOverlay?.ShowCardDetails(this, Beatmap);
        }

        protected override bool OnHover(HoverEvent e)
        {
            preview?.Start();

            return base.OnHover(e);
        }

        private partial class CardBorder : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load(CardColours colours)
            {
                RelativeSizeAxes = Axes.Both;
                Masking = true;
                CornerRadius = RankedPlayCard.CORNER_RADIUS;
                BorderThickness = 1.5f;
                BorderColour = ColourInfo.GradientVertical(colours.Border.Opacity(0.5f), colours.Border.Opacity(0));

                InternalChild = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true,
                    EdgeSmoothness = new Vector2(3),
                };
            }
        }

        private partial class PreviewVisualization(APIBeatmap beatmap) : CompositeDrawable
        {
            private ScheduledDelegate? pulseDelegate;

            private Drawable border = null!;

            [Resolved]
            private CardColours colours { get; set; } = null!;

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.Both;
                AlwaysPresent = true;
                Alpha = 0;

                AddInternal(border = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(-1.5f),
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = RankedPlayCard.CORNER_RADIUS + 1.5f,
                        Blending = BlendingParameters.Additive,
                        BorderThickness = 2f,
                        BorderColour = colours.Border.Opacity(0.5f),
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Colour = colours.Border.Opacity(0.1f),
                            Type = EdgeEffectType.Glow,
                            Radius = 25f,
                        },
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true,
                            EdgeSmoothness = new Vector2(3),
                        },
                    }
                });
            }

            protected override void Update()
            {
                base.Update();

                if (card?.ShowSelectionOutline == true)
                    border.Alpha = 0;
                else
                    border.Alpha = 1;
            }

            private void pulse()
            {
                var expandingBorder = new Container
                {
                    Size = DrawSize,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = RankedPlayCard.CORNER_RADIUS,
                    BorderThickness = 2,
                    BorderColour = colours.Border,
                    Alpha = 0,
                    Blending = BlendingParameters.Additive,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        AlwaysPresent = true,
                    }
                };
                AddInternal(expandingBorder);

                const float expansion = 20;

                using (BeginDelayedSequence(150))
                {
                    expandingBorder
                        .FadeInFromZero(200)
                        .Then()
                        .FadeOut(1000);

                    expandingBorder.ResizeTo(DrawSize + new Vector2(expansion), 1000, Easing.OutQuart)
                                   .TransformTo(nameof(CornerRadius), RankedPlayCard.CORNER_RADIUS + expansion / 2, 1000, Easing.OutQuart)
                                   .TransformTo(nameof(BorderThickness), 0f, 1300, Easing.In)
                                   .Expire();
                }

                card?.Pulse();
            }

            [Resolved]
            private RankedPlayCard? card { get; set; }

            public void PreviewStarted() => Schedule(() =>
            {
                this.FadeIn(100);

                double interval = 1500;

                if (beatmap.BPM > 50)
                {
                    interval = (60_000 / beatmap.BPM) * 4;

                    while (interval < 800)
                        interval *= 2;

                    while (interval > 1600)
                        interval /= 2;
                }

                pulseDelegate?.Cancel();
                pulseDelegate = Scheduler.AddDelayed(pulse, interval, true);
                pulse();
            });

            public void PreviewStopped() => Schedule(() =>
            {
                FinishTransforms(true);
                this.FadeOut(200);

                pulseDelegate?.Cancel();
                pulseDelegate = null;
            });
        }

        [Resolved]
        private BeatmapSetOverlay? beatmapSetOverlay { get; set; }

        public MenuItem[] ContextMenuItems =>
        [
            new OsuMenuItem(ContextMenuStrings.ViewBeatmap, MenuItemType.Highlighted, () => beatmapSetOverlay?.ShowBeatmapSet(Beatmap.BeatmapSet))
        ];
    }
}
