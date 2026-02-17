// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Audio;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCard
    {
        public partial class SongPreviewContainer : Container
        {
            protected override Container<Drawable> Content { get; }

            public SongPreviewContainer()
            {
                InternalChildren =
                [
                    Content = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                ];
            }

            [Resolved]
            private PreviewTrackManager previewTrackManager { get; set; } = null!;

            private PreviewTrack? previewTrack;

            public void LoadPreview(APIBeatmap beatmap)
            {
                Debug.Assert(previewTrack == null);

                LoadComponentAsync(previewTrack = previewTrackManager.Get(beatmap.BeatmapSet!), track =>
                {
                    AddInternal(track);

                    if (IsHovered)
                        startPreviewIfAvailable();
                });
            }

            protected override bool OnHover(HoverEvent e)
            {
                startPreviewIfAvailable();

                return base.OnHover(e);
            }

            private void startPreviewIfAvailable() => previewTrack?.Start();
        }
    }
}
