// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osuTK;
using osuTK.Graphics;
using Vector4 = System.Numerics.Vector4;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardFrontSide : CompositeDrawable
    {
        private readonly APIBeatmap beatmap;

        public RankedPlayCardFrontSide(APIBeatmap beatmap)
        {
            this.beatmap = beatmap;
            Size = new Vector2(120, 200);
        }

        [Resolved]
        private OsuColour colour { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            Masking = true;
            CornerRadius = 4;

            var accentColour = colour.ForStarDifficulty(beatmap.StarRating);
            var onAccentColour = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : colourProvider.Background5;

            InternalChildren =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = backgroundColour()
                },
                new Container
                {
                    Size = new Vector2(120),
                    Children =
                    [
                        new BufferedContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            GrayscaleStrength = 0.25f,
                            FrameBufferScale = new Vector2(1f),
                            Child = new UpdateableOnlineBeatmapSetCover(timeBeforeLoad: 0)
                            {
                                OnlineInfo = beatmap.BeatmapSet,
                                RelativeSizeAxes = Axes.Both,
                            },
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex("#222228").Opacity(0.2f), Color4Extensions.FromHex("#222228").Opacity(0.65f))
                        },
                        new OsuTextFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            TextAnchor = Anchor.BottomLeft,
                            Padding = new MarginPadding(5),
                            ParagraphSpacing = 0,
                        }.With(d =>
                        {
                            d.AddParagraph(new RomanisableString(beatmap.Metadata.TitleUnicode, beatmap.Metadata.Title), static s => s.Font = OsuFont.GetFont(size: 12, weight: FontWeight.SemiBold));
                            d.AddParagraph(new RomanisableString(beatmap.Metadata.ArtistUnicode, beatmap.Metadata.Artist), static s => s.Font = OsuFont.GetFont(size: 9, weight: FontWeight.SemiBold));
                        }),
                        new FillFlowContainer
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Children =
                            [
                                new Container
                                {
                                    AutoSizeAxes = Axes.Y,
                                    Width = 114,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Margin = new MarginPadding { Top = 3 },
                                    Masking = true,
                                    CornerRadius = 2,
                                    Children =
                                    [
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = accentColour,
                                        },
                                        new GridContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            ColumnDimensions = [new Dimension(GridSizeMode.AutoSize), new Dimension()],
                                            RowDimensions = [new Dimension(GridSizeMode.AutoSize)],
                                            Padding = new MarginPadding { Horizontal = 2, Vertical = 1 },
                                            Content = new Drawable[][]
                                            {
                                                [
                                                    new Stars(beatmap.StarRating)
                                                    {
                                                        Colour = onAccentColour,
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        StarSize = 6,
                                                    },
                                                    new OsuTextFlowContainer(s => s.Font = OsuFont.GetFont(size: 10, weight: FontWeight.SemiBold))
                                                    {
                                                        AutoSizeAxes = Axes.Y,
                                                        RelativeSizeAxes = Axes.X,
                                                        Text = beatmap.DifficultyName,
                                                        Colour = onAccentColour,
                                                        TextAnchor = Anchor.CentreRight,
                                                    },
                                                ]
                                            }
                                        }
                                    ]
                                },
                                new OsuTextFlowContainer(s => s.Font = OsuFont.GetFont(size: 10, weight: FontWeight.SemiBold))
                                {
                                    Text = $"mapped by {beatmap.Metadata.Author.Username}",
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    TextAnchor = Anchor.TopCentre,
                                    Width = 100,
                                },
                            ]
                        }
                    ]
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 4,
                    BorderThickness = 1.5f,
                    BorderColour = ColourInfo.GradientVertical(
                        accentColour.Opacity(0.5f),
                        accentColour.Opacity(0f)
                    ),
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        AlwaysPresent = true,
                        EdgeSmoothness = new Vector2(3)
                    }
                },
                new CircularContainer
                {
                    Width = 60,
                    Height = 2,
                    Position = new Vector2(60, 120),
                    Origin = Anchor.Centre,
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = accentColour
                    }
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(7) { Top = 130 },
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(6),
                    Children =
                    [
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children =
                            [
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Spacing = new Vector2(4),
                                    Children =
                                    [
                                        new OsuSpriteText
                                        {
                                            Text = "Length",
                                            Font = OsuFont.GetFont(size: 9, weight: FontWeight.Medium),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            UseFullGlyphHeight = false,
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = beatmap.HitLength.ToFormattedDuration(),
                                            Font = OsuFont.GetFont(size: 9, weight: FontWeight.SemiBold),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            UseFullGlyphHeight = false,
                                        },
                                    ]
                                },

                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Spacing = new Vector2(4),
                                    Children =
                                    [
                                        new OsuSpriteText
                                        {
                                            Text = "BPM",
                                            Font = OsuFont.GetFont(size: 9, weight: FontWeight.Medium),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            UseFullGlyphHeight = false,
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = ((int)beatmap.BPM).ToString(),
                                            Font = OsuFont.GetFont(size: 9, weight: FontWeight.SemiBold),
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            UseFullGlyphHeight = false,
                                        },
                                    ]
                                },
                            ]
                        },
                        new StatisticsRow
                        {
                            Label = "HP Drain",
                            Value = beatmap.DrainRate.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.DrainRate / 10f,
                            AccentColour = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : accentColour,
                        },
                        new StatisticsRow
                        {
                            Label = "Circle Size",
                            Value = beatmap.CircleSize.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.CircleSize / 10f,
                            AccentColour = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : accentColour,
                        },
                        new StatisticsRow
                        {
                            Label = "Approach Rate",
                            Value = beatmap.ApproachRate.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.ApproachRate / 10f,
                            AccentColour = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : accentColour,
                        },
                        new StatisticsRow
                        {
                            Label = "Accuracy",
                            Value = beatmap.OverallDifficulty.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.OverallDifficulty / 10f,
                            AccentColour = beatmap.StarRating >= OsuColour.STAR_DIFFICULTY_DEFINED_COLOUR_CUTOFF ? colour.Orange1 : accentColour,
                        },
                    ]
                },
            ];
        }

        private Colour4 backgroundColour()
        {
            var hsl = ((Colour4)this.colour.ForStarDifficulty(beatmap.StarRating)).ToHSL();

            var main = Colour4.FromHex("#222228");

            return main * new Colour4(new Vector4(0.5f)) + Colour4.FromHSL(hsl.X, hsl.Y * 0.1f, 0.15f) * new Colour4(new Vector4(0.5f));
        }

        private partial class Stars(double starRating) : CompositeDrawable
        {
            public float StarSize { get; init; } = 8;

            [BackgroundDependencyLoader]
            private void load()
            {
                FillFlowContainer flow;

                AutoSizeAxes = Axes.Both;
                InternalChild = flow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                };

                for (int i = 1; i < starRating; i++)
                {
                    flow.Add(new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Star,
                        Size = new Vector2(StarSize),
                    });
                }

                float lastStarWidth = (int)((starRating % 1) * 4) / 4f;

                if (lastStarWidth > 0)
                {
                    flow.Add(new Container
                    {
                        Size = new Vector2(StarSize * lastStarWidth, StarSize),
                        Masking = true,
                        Child = new SpriteIcon
                        {
                            Icon = FontAwesome.Solid.Star,
                            Size = new Vector2(StarSize),
                        }
                    });
                }
            }
        }

        private partial class StatisticsRow : CompositeDrawable
        {
            public required LocalisableString Label { get; init; }

            public required LocalisableString Value { get; init; }

            public required float NormalizedValue { get; init; }

            public required Color4 AccentColour { get; init; }

            [BackgroundDependencyLoader]
            private void load(OsuColour colour)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;
                InternalChildren =
                [
                    new OsuSpriteText
                    {
                        Text = Label,
                        Font = OsuFont.GetFont(size: 9, weight: FontWeight.Medium),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        UseFullGlyphHeight = false,
                    },
                    new OsuSpriteText
                    {
                        RelativePositionAxes = Axes.X,
                        Text = Value,
                        Font = OsuFont.GetFont(size: 9, weight: FontWeight.SemiBold),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreRight,
                        UseFullGlyphHeight = false,
                        X = 0.65f,
                        Padding = new MarginPadding { Right = 2 }
                    },
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Width = 0.35f,
                        Height = 2,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Masking = true,
                        Children =
                        [
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.06f
                            },
                            new CircularContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = NormalizedValue,
                                Masking = true,
                                Children =
                                [
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = AccentColour,
                                    },
                                ]
                            }
                        ]
                    }
                ];
            }
        }
    }
}
