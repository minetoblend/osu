// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osu.Game.Database;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public partial class RankedPlayCard : CompositeDrawable
    {
        public static readonly Vector2 SIZE = new Vector2(120, 200);

        public static readonly float CORNER_RADIUS = 8;

        public readonly RankedPlayCardWithPlaylistItem Item;

        private readonly IBindable<MultiplayerPlaylistItem?> playlistItem;

        private readonly Container content;
        private readonly Container cardContent;
        private readonly SelectionOutline selectionOutline;

        public bool ShowSelectionOutline
        {
            set => selectionOutline.FadeTo(value ? 1 : 0, 50);
        }

        public float Elevation;

        public float TiltX;
        public float TiltY;
        private float cardRevealTilt = -MathF.PI;

        [Resolved]
        private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

        private readonly RankedPlayCardBackSide backSide;
        private RankedPlayCardContent? frontSide;

        public RankedPlayCard(RankedPlayCardWithPlaylistItem item)
        {
            Item = item;

            Size = SIZE;

            playlistItem = item.PlaylistItem.GetBoundCopy();

            const float padding = 40;

            InternalChildren =
            [
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding(-padding),
                    Child = tiltContainer = new TiltContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(padding),
                        BackgroundColour = Color4Extensions.FromHex("72D5FF").Opacity(0f),
                        Children =
                        [
                            cardContent = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children =
                                [
                                    backSide = new RankedPlayCardBackSide()
                                ]
                            },
                            selectionOutline = new SelectionOutline
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0,
                            }
                        ]
                    }
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            playlistItem.BindValueChanged(e => onPlaylistItemChanged(e.NewValue));
            if (playlistItem.Value != null)
                loadCardContent(playlistItem.Value, false);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            float tilt = TiltX + cardRevealTilt;

            var rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), tilt) *
                           Quaternion.FromAxisAngle(new Vector3(1, 0, 0), TiltY);

            bool flipped = false;

            if (Vector3.TransformNormal(Vector3.UnitZ, Matrix4.CreateFromQuaternion(rotation)).Z < 0)
            {
                flipped = true;
                rotation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), MathF.PI) * rotation;
            }

            tiltContainer.PerspectiveRotation = rotation;

            if (frontSide == null || flipped)
            {
                frontSide?.Hide();
                backSide.Show();
            }
            else
            {
                frontSide?.Show();
                backSide.Hide();
            }
        }

        #region beatmap fetching logic & card flip

        private readonly TaskCompletionSource cardRevealed = new TaskCompletionSource();
        private readonly TiltContainer tiltContainer;

        public Task CardRevealed => cardRevealed.Task;

        private void onPlaylistItemChanged(MultiplayerPlaylistItem? playlistItem)
        {
            if (playlistItem != null)
                loadCardContent(playlistItem, true);
        }

        private void loadCardContent(MultiplayerPlaylistItem playlistItem, bool flip) => Task.Run(async () =>
        {
            var beatmap = await beatmapLookupCache.GetBeatmapAsync(playlistItem.BeatmapID).ConfigureAwait(false);

            cardRevealed.TrySetResult();

            if (beatmap == null)
            {
                Logger.Log($"Failed to load beatmap {playlistItem.BeatmapID} for playlistItem {playlistItem.ID}.", level: LogLevel.Error);
                return;
            }

            Schedule(() => SetContent(new RankedPlayCardContent(beatmap), flip));
        });

        public void SetContent(RankedPlayCardContent newContent, bool flip)
        {
            frontSide?.Expire();
            cardContent.Add(frontSide = newContent);

            if (!flip)
            {
                cardRevealTilt = 0;
                return;
            }

            this.TransformTo(nameof(cardRevealTilt), -MathF.PI)
                .TransformTo(nameof(cardRevealTilt), -MathF.PI / 2, 150, Easing.In)
                .Then()
                .TransformTo(nameof(cardRevealTilt), 0f, 600, Easing.OutElasticQuarter);
        }

        #endregion

        public void PopOutAndExpire()
        {
            content.ScaleTo(0, 500, Easing.In);

            this.FadeOut(500)
                .Expire();
        }

        private partial class SelectionOutline : CompositeDrawable
        {
            [BackgroundDependencyLoader]
            private void load()
            {
                const float border_width = 4;

                InternalChild = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(-1),
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = CORNER_RADIUS,
                        BorderThickness = border_width,
                        BorderColour = Color4Extensions.FromHex("c4eeff"),
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Glow,
                            Radius = 30,
                            Colour = Color4Extensions.FromHex("72D5FF").Opacity(0.5f),
                            Hollow = true,
                            Roundness = 10
                        },
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        }
                    }
                };
            }
        }
    }
}
