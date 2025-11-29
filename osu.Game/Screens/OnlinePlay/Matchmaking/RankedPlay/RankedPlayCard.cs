// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayCard : CompositeDrawable
    {
        private const float shiny_alpha = 0.2f;

        private static FontUsage heading0 => OsuFont.GetFont(size: 24, weight: FontWeight.Bold);

        private readonly APIBeatmap beatmap;

        private FillFlowContainer<UserTagPill> userTags = null!;
        private BufferedContainer baseBackground = null!;
        private BufferedContainer contentBackground = null!;
        private Drawable shine = null!;

        private bool shiny;

        public RankedPlayCard(APIBeatmap beatmap)
        {
            this.beatmap = beatmap;

            Size = new Vector2(300, 500);
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            Masking = true;
            CornerRadius = 20;

            InternalChildren = new Drawable[]
            {
                baseBackground = new BufferedContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    BackgroundColour = colourProvider.Background1,
                    Child = shine = new Box
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = Shiny ? shiny_alpha : 0,
                        Colour = new ColourInfo
                        {
                            TopLeft = new Color4(255, 255, 100, 255),
                            TopRight = new Color4(50, 220, 255, 255),
                            BottomLeft = new Color4(255, 100, 220, 255),
                            BottomRight = new Color4(255, 255, 100, 255),
                        }
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(10),
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 10,
                        Children = new Drawable[]
                        {
                            contentBackground = new BufferedContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    baseBackground.CreateView().With(d =>
                                    {
                                        d.RelativeSizeAxes = Axes.Both;
                                    }),
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = colourProvider.Background5,
                                        Alpha = 0.5f
                                    },
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Height = 0.7f,
                                Children = new Drawable[]
                                {
                                    new UpdateableOnlineBeatmapSetCover
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        OnlineInfo = beatmap.BeatmapSet,
                                        Colour = ColourInfo.GradientVertical(Color4.White.Opacity(0.1f), Color4.White.Opacity(0.3f))
                                    },
                                    new FillFlowContainer
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Y = 60,
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Vertical,
                                        Children = new Drawable[]
                                        {
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                Text = beatmap.Metadata.Artist,
                                                Font = OsuFont.Style.Body
                                            },
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                Text = beatmap.Metadata.Title,
                                                Font = heading0.With(weight: FontWeight.SemiBold)
                                            },
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                Text = beatmap.DifficultyName,
                                                Font = OsuFont.Style.Body
                                            },
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.TopCentre,
                                                Origin = Anchor.TopCentre,
                                                Text = $"mapped by {beatmap.Metadata.Author.Username}",
                                                Font = OsuFont.Style.Caption2
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        Anchor = Anchor.BottomCentre,
                                        Origin = Anchor.BottomCentre,
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Padding = new MarginPadding { Horizontal = 8 },
                                        Margin = new MarginPadding { Bottom = 8 },
                                        Child = new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            ColumnDimensions =
                                            [
                                                new Dimension(GridSizeMode.AutoSize),
                                                new Dimension(GridSizeMode.Absolute, 8),
                                                new Dimension(),
                                                new Dimension(GridSizeMode.Absolute, 8),
                                                new Dimension()
                                            ],
                                            RowDimensions =
                                            [
                                                new Dimension(GridSizeMode.AutoSize)
                                            ],
                                            Content = new[]
                                            {
                                                new Drawable?[]
                                                {
                                                    new StarRatingDisplay(new StarDifficulty(beatmap.StarRating, 0))
                                                    {
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                    },
                                                    null,
                                                    new DetailsPill("Length:", TimeSpan.FromMilliseconds(beatmap.HitLength).ToFormattedDuration())
                                                    {
                                                        Background = contentBackground.CreateView(),
                                                        RelativeSizeAxes = Axes.X,
                                                        Height = 22
                                                    },
                                                    null,
                                                    new DetailsPill("BPM:", FormatUtils.RoundBPM(beatmap.BPM).ToLocalisableString(@"0.##"))
                                                    {
                                                        Background = contentBackground.CreateView(),
                                                        RelativeSizeAxes = Axes.X,
                                                        Height = 22
                                                    },
                                                }
                                            }
                                        }
                                    },
                                }
                            },
                            new FillFlowContainer
                            {
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre,
                                RelativeSizeAxes = Axes.Both,
                                Height = 0.3f,
                                Padding = new MarginPadding(8),
                                Spacing = new Vector2(8),
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    new OsuSpriteText
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Text = $"User Rating {beatmap.BeatmapSet!.Ratings.Average():0.00}",
                                        Font = OsuFont.Style.Body.With(weight: FontWeight.Bold)
                                    },
                                    new UserRatingDisplay
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        RelativeSizeAxes = Axes.X,
                                        Margin = new MarginPadding { Top = -28 },
                                        Data = beatmap.BeatmapSet!.Ratings,
                                    },
                                    userTags = new FillFlowContainer<UserTagPill>
                                    {
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Spacing = new Vector2(4),
                                    }
                                }
                            },
                            new Container
                            {
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight,
                                Size = new Vector2(145, 50),
                                Margin = new MarginPadding
                                {
                                    Top = -20,
                                    Right = -20
                                },
                                Masking = true,
                                CornerRadius = 20,
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Children = new Drawable[]
                                        {
                                            baseBackground.CreateView().With(d =>
                                            {
                                                d.RelativeSizeAxes = Axes.Both;
                                                d.SynchronisedDrawQuad = true;
                                            }),
                                            new OsuSpriteText
                                            {
                                                Anchor = Anchor.BottomCentre,
                                                Origin = Anchor.BottomCentre,
                                                Position = new Vector2(-8, -10),
                                                Font = heading0.With(weight: FontWeight.Bold),
                                                Colour = Color4.Black,
                                                Alpha = 0.5f,
                                                Text = "Season 1",
                                                UseFullGlyphHeight = false
                                            }
                                        }
                                    }
                                }
                            },
                        }
                    }
                }
            };

            updateUserTags();
        }

        public bool Shiny
        {
            get => shiny;
            set
            {
                shiny = value;

                if (LoadState >= LoadState.Ready)
                    shine.Alpha = value ? shiny_alpha : 0;
            }
        }

        private void updateUserTags()
        {
            if (beatmap.TopTags == null || beatmap.TopTags.Length == 0 || beatmap.BeatmapSet!.RelatedTags == null)
                return;

            var tagsById = beatmap.BeatmapSet.RelatedTags.ToDictionary(t => t.Id);
            userTags.Children = beatmap.TopTags
                                       .Select(t => (topTag: t, relatedTag: tagsById.GetValueOrDefault(t.TagId)))
                                       .Where(t => t.relatedTag != null)
                                       // see https://github.com/ppy/osu-web/blob/bb3bd2e7c6f84f26066df5ea20a81c77ec9bb60a/resources/js/beatmapsets-show/controller.ts#L103-L106 for sort criteria
                                       .OrderByDescending(t => t.topTag.VoteCount)
                                       .ThenBy(t => t.relatedTag!.Name)
                                       .Select(t => new UserTagPill(t.relatedTag!)
                                       {
                                           Background = baseBackground.CreateView()
                                       })
                                       .ToArray();
        }
    }
}
