// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Screens.SelectV2;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components
{
    public partial class BeatmapDetailOverlay
    {
        public partial class MetadataWedge : CompositeDrawable
        {
            public required IBindable<APIBeatmap?> Beatmap { get; init; }

            private BeatmapMetadataWedge.MetadataDisplay userTags = null!;
            private BeatmapMetadataWedge.MetadataDisplay mapperTags = null!;

            public MetadataWedge()
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                Width = 0.9f;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                InternalChild = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0f, 4f),
                    Shear = OsuGame.SHEAR,
                    Children = new[]
                    {
                        new ShearAligningWrapper(new Container
                        {
                            CornerRadius = 10,
                            Masking = true,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Children = new Drawable[]
                            {
                                new WedgeBackground(),
                                new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Shear = -OsuGame.SHEAR,
                                    Padding = new MarginPadding { Left = SongSelect.WEDGE_CONTENT_MARGIN, Right = 35, Vertical = 16 },
                                    Children = new Drawable[]
                                    {
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0f, 10f),
                                            Children = new Drawable[]
                                            {
                                                userTags = new BeatmapMetadataWedge.MetadataDisplay(BeatmapsetsStrings.ShowInfoUserTags),
                                                mapperTags = new BeatmapMetadataWedge.MetadataDisplay(BeatmapsetsStrings.ShowInfoMapperTags),
                                            },
                                        },
                                    },
                                },
                            },
                        }),
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Beatmap.BindValueChanged(e =>
                {
                    if (e.NewValue is not { } beatmap)
                        return;

                    var metadata = beatmap.Metadata;

                    if (!string.IsNullOrEmpty(metadata.Tags))
                        mapperTags.Tags = (metadata.Tags.Split(' '), _ => { });
                    else
                        mapperTags.Tags = (Array.Empty<string>(), _ => { });

                    string[] topUserTags = beatmap.GetTopUserTags().Select(s => s.Tag.Name).ToArray();

                    userTags.Tags = (topUserTags, _ => { });

                    if (topUserTags.Length > 0)
                        userTags.Show();
                }, true);
            }
        }
    }
}
