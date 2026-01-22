// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Extensions;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCardContent
    {
        private partial class CardStats(APIBeatmap beatmap) : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(5),
                    Padding = new MarginPadding(7),
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
                        },
                        new StatisticsRow
                        {
                            Label = "Circle Size",
                            Value = beatmap.CircleSize.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.CircleSize / 10f,
                        },
                        new StatisticsRow
                        {
                            Label = "Approach Rate",
                            Value = beatmap.ApproachRate.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.ApproachRate / 10f,
                        },
                        new StatisticsRow
                        {
                            Label = "Accuracy",
                            Value = beatmap.OverallDifficulty.ToStandardFormattedString(maxDecimalDigits: 1),
                            NormalizedValue = beatmap.OverallDifficulty / 10f,
                        },
                    ]
                };
            }
        }

        private partial class StatisticsRow : CompositeDrawable
        {
            public required LocalisableString Label { get; init; }

            public required LocalisableString Value { get; init; }

            public required float NormalizedValue { get; init; }

            [BackgroundDependencyLoader]
            private void load(CardColours colours)
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
                        Padding = new MarginPadding { Right = 2 },
                        Colour = colours.OnBackground,
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
                                Colour = colours.BackgroundLightest,
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
                                        Colour = colours.PrimaryWithContrastToBackground,
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
