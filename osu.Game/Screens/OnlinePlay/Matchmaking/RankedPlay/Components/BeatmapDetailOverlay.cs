// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Threading;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Screens.SelectV2;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components
{
    public partial class BeatmapDetailOverlay : VisibilityContainer
    {
        private readonly Bindable<APIBeatmap?> beatmap = new Bindable<APIBeatmap?>();

        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Blue);

        public APIBeatmap? Beatmap
        {
            get => beatmap.Value;
            set => beatmap.Value = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children =
            [
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Shear = OsuGame.SHEAR,
                    Scale = new Vector2(0.8f),
                    Padding = new MarginPadding
                    {
                        Left = -SongSelect.CORNER_RADIUS_HIDE_OFFSET,
                    },
                    Child = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Spacing = new Vector2(0f, 4f),
                        Direction = FillDirection.Vertical,
                        Children =
                        [
                            new ShearAligningWrapper(new TitleWedge { Beatmap = beatmap.GetBoundCopy() })
                            {
                                Shear = -OsuGame.SHEAR,
                            },
                            new ShearAligningWrapper(new MetadataWedge { Beatmap = beatmap.GetBoundCopy() })
                            {
                                Shear = -OsuGame.SHEAR,
                            },
                        ]
                    }
                }
            ];
        }

        private ScheduledDelegate? hideDelegate;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            beatmap.BindValueChanged(e =>
            {
                hideDelegate?.Cancel();

                if (e.NewValue != null)
                {
                    Show();
                    populateContent(e.NewValue);
                }
                else
                {

                    hideDelegate = Scheduler.AddDelayed(Hide, 500);
                }
            });
        }

        private void populateContent(APIBeatmap beatmap)
        {
        }

        protected override void PopIn()
        {
            this.FadeIn(300)
                .MoveToX(0, 400, Easing.OutExpo);
        }

        protected override void PopOut()
        {
            this.FadeOut(200)
                .MoveToX(-80, 400, Easing.InQuad);
        }
    }
}
