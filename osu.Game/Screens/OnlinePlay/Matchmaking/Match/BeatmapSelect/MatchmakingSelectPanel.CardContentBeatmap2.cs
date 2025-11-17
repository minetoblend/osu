// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Beatmaps.Drawables.Cards;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Localisation;
using osu.Game.Online;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.BeatmapSet;
using osu.Game.Rulesets.Mods;
using osu.Game.Screens.Ranking;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanel
    {
        public partial class CardContentBeatmap2 : CardContent, IHasContextMenu
        {
            public override AvatarOverlay SelectionOverlay => selectionOverlay;

            [Resolved]
            private OverlayColourProvider colourProvider { get; set; } = null!;

            [Resolved]
            private BeatmapSetOverlay? beatmapSetOverlay { get; set; }

            private readonly IBindable<DownloadState> downloadState = new Bindable<DownloadState>();
            private readonly IBindableNumber<double> downloadProgress = new BindableDouble();
            private readonly Bindable<BeatmapSetFavouriteState> favouriteState = new Bindable<BeatmapSetFavouriteState>();
            private readonly APIBeatmapSet beatmapSet;
            private readonly APIBeatmap beatmap;
            private readonly Mod[] mods;

            private BeatmapCardThumbnail thumbnail = null!;
            private CollapsibleButtonContainer buttonContainer = null!;
            private FillFlowContainer idleBottomContent = null!;
            private BeatmapCardDownloadProgressBar downloadProgressBar = null!;
            private AvatarOverlay selectionOverlay = null!;
            private SplitPanel card = null!;

            public CardContentBeatmap2(APIBeatmap beatmap, Mod[] mods)
            {
                this.beatmap = beatmap;
                this.mods = mods;

                beatmapSet = beatmap.BeatmapSet!;
                favouriteState.Value = new BeatmapSetFavouriteState(beatmapSet.HasFavourited, beatmapSet.FavouriteCount);
            }

            private Container content = null!;

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                InternalChild = card = new SplitPanel
                {
                    RelativeSizeAxes = Axes.Both,
                    Background =
                    {
                        Colour = colours.ForStarDifficulty(beatmap.StarRating).Darken(0.8f),
                    },
                    Children = new Drawable[]
                    {
                        new DelayedLoadWrapper(timeBeforeLoad: 0, createFunc: () => new OnlineBeatmapSetCover(beatmapSet)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill,
                        }.With(d => d.OnLoadComplete += _ => d.FadeInFromZero(150))),
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.25f,
                            Colour = Color4Extensions.FromHex("2A2226"),
                            Alpha = 0.8f
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 1 - 0.25f,
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Colour = ColourInfo.GradientHorizontal(Color4Extensions.FromHex("2A2226"), Color4Extensions.FromHex("2A2226").Opacity(0.3f)),
                            Alpha = 0.8f
                        },
                        content = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = 14, Vertical = 7 },
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(8),
                                    Children = new Drawable[]
                                    {
                                        new SpotlightBeatmapBadge
                                        {
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight,
                                        },
                                    }
                                },
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Text = new RomanisableString(beatmap.Metadata.TitleUnicode, beatmap.Metadata.Title),
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = new RomanisableString(beatmap.Metadata.ArtistUnicode, beatmap.Metadata.Artist),
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12),
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = $"mapped by {beatmap.Metadata.Author.Username}",
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 9),
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight,
                                            Scale = new Vector2(0.6f),
                                            Spacing = new Vector2(4),
                                            Margin = new MarginPadding { Top = 20 },
                                            Children = new[]
                                            {
                                                new UserTagControl.DrawableUserTag(new UserTag(new APITag
                                                {
                                                    Name = "Foo/Bar",
                                                })),
                                                new UserTagControl.DrawableUserTag(new UserTag(new APITag
                                                {
                                                    Name = "Foo/Bar"
                                                })),
                                                new UserTagControl.DrawableUserTag(new UserTag(new APITag
                                                {
                                                    Name = "Foo/Bar"
                                                }))
                                            }
                                        }
                                    }
                                },
                                attributes = new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Spacing = new Vector2(7),
                                    Alpha = 0,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Padding = new MarginPadding { Horizontal = 6 },
                                    Y = -10,
                                    Children = new Drawable[]
                                    {
                                        new OsuSpriteText
                                        {
                                            Text = $"HP: {beatmap.Difficulty.DrainRate}",
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = $"CS: {beatmap.Difficulty.CircleSize}",
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = $"AR: {beatmap.Difficulty.ApproachRate}",
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                                        },
                                        new OsuSpriteText
                                        {
                                            Text = $"OD: {beatmap.Difficulty.OverallDifficulty}",
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 14),
                                        },
                                    }
                                },
                                selectionOverlay = new AvatarOverlay
                                {
                                    Anchor = Anchor.BottomRight,
                                    Origin = Anchor.BottomRight,
                                    Y = 8,
                                },
                                bottomRow = new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Spacing = new Vector2(7),
                                    Children = new Drawable[]
                                    {
                                        new StarRatingDisplay(new StarDifficulty(beatmap.StarRating, 0), StarRatingDisplaySize.Small, animated: true)
                                        {
                                            Origin = Anchor.CentreLeft,
                                            Anchor = Anchor.CentreLeft,
                                            Scale = new Vector2(0.9f),
                                        },
                                        new OsuSpriteText
                                        {
                                            Origin = Anchor.CentreLeft,
                                            Anchor = Anchor.CentreLeft,
                                            Text = beatmap.DifficultyName,
                                            Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12)
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                card.BackgroundLayer.Add(new TrianglesV2
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.4f,
                    Colour = ColourInfo.GradientVertical(Color4.White, Color4.Transparent),
                    Blending = BlendingParameters.Additive,
                });

                card.BackgroundLayer.Add(new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Horizontal = 13, Vertical = 3 },
                    Children = new Drawable[]
                    {
                        bottomRow2 = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Spacing = new Vector2(7),
                            Y = -7,
                            Children = new Drawable[]
                            {
                                new Container
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Scale = new Vector2(0.9f),
                                    Child = new GridContainer
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        AutoSizeAxes = Axes.Both,
                                        Margin = new MarginPadding { Horizontal = 7f },
                                        ColumnDimensions = new[]
                                        {
                                            new Dimension(GridSizeMode.AutoSize),
                                            new Dimension(GridSizeMode.Absolute, 3f),
                                            new Dimension(GridSizeMode.AutoSize, minSize: 25f),
                                        },
                                        RowDimensions = new[] { new Dimension(GridSizeMode.AutoSize) },
                                        Content = new[]
                                        {
                                            new[]
                                            {
                                                new SpriteIcon
                                                {
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Icon = FontAwesome.Solid.Star,
                                                    Size = new Vector2(8f),
                                                },
                                                Empty(),
                                                new OsuSpriteText
                                                {
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    Margin = new MarginPadding { Bottom = 1.5f },
                                                    Spacing = new Vector2(-1.4f),
                                                    Font = OsuFont.Torus.With(size: 14.4f, weight: FontWeight.Bold, fixedWidth: true),
                                                    Shadow = false,
                                                    Text = beatmap.StarRating.FormatStarRating()
                                                },
                                            }
                                        }
                                    },
                                },
                                new OsuSpriteText
                                {
                                    Origin = Anchor.CentreLeft,
                                    Anchor = Anchor.CentreLeft,
                                    Text = beatmap.DifficultyName,
                                    Font = OsuFont.GetFont(weight: FontWeight.SemiBold, size: 12)
                                },
                            }
                        }
                    }
                });

                if (RNG.NextSingle() < 0.2)
                {
                    selectionOverlay.AddUser(new APIUser
                    {
                        Id = 4,
                        Username = "Foo"
                    });
                }

                if (RNG.NextSingle() < 0.2)
                {
                    selectionOverlay.AddUser(new APIUser
                    {
                        Id = 2,
                        Username = "Foo"
                    });
                }

                if (RNG.NextSingle() < 0.2)
                {
                    selectionOverlay.AddUser(new APIUser
                    {
                        Id = 3,
                        Username = "Foo"
                    });
                }
            }

            private FillFlowContainer bottomRow = null!;
            private FillFlowContainer bottomRow2 = null!;
            private FillFlowContainer attributes = null!;

            protected override bool OnHover(HoverEvent e)
            {
                card.TransformTo(nameof(card.ContentPadding), 20f, 300, Easing.OutExpo);

                this.ResizeHeightTo(1.2f, 300, Easing.OutExpo);

                bottomRow.MoveToY(25, 300, Easing.OutExpo);
                bottomRow2.MoveToY(0, 300, Easing.OutExpo);
                attributes
                    .MoveToY(0, 300, Easing.OutExpo)
                    .FadeIn(300, Easing.Out);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                this.ResizeHeightTo(1f, 300, Easing.OutExpo);

                bottomRow.MoveToY(0, 300, Easing.OutExpo);
                bottomRow2.MoveToY(-7, 300, Easing.OutExpo);
                attributes
                    .MoveToY(-15, 300, Easing.OutExpo)
                    .FadeOut(150, Easing.Out);

                card.TransformTo(nameof(card.ContentPadding), 3f, 300, Easing.OutExpo);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                downloadState.BindValueChanged(_ => updateState(), true);

                FinishTransforms(true);
            }

            private void updateState()
            {
                return;

                bool showDetails = IsHovered;

                buttonContainer.ShowDetails.Value = showDetails;
                thumbnail.Dimmed.Value = showDetails;

                bool showProgress = downloadState.Value == DownloadState.Downloading || downloadState.Value == DownloadState.Importing;

                idleBottomContent.FadeTo(showProgress ? 0 : 1, 340, Easing.OutQuint);
                downloadProgressBar.FadeTo(showProgress ? 1 : 0, 340, Easing.OutQuint);
            }

            public MenuItem[] ContextMenuItems
            {
                get
                {
                    List<MenuItem> items = new List<MenuItem>
                    {
                        new OsuMenuItem(ContextMenuStrings.ViewBeatmap, MenuItemType.Highlighted, () => beatmapSetOverlay?.FetchAndShowBeatmap(beatmap.OnlineID))
                    };

                    foreach (var button in buttonContainer.Buttons)
                    {
                        if (button.Enabled.Value)
                            items.Add(new OsuMenuItem(button.TooltipText.ToSentence(), MenuItemType.Standard, () => button.TriggerClick()));
                    }

                    return items.ToArray();
                }
            }
        }
    }
}
