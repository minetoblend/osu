// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Database;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen
    {
        public partial class Card : CompositeDrawable
        {
            public readonly Bindable<bool> AllowSelection = new Bindable<bool>();
            public readonly BindableBool Selected = new BindableBool();

            public readonly RankedPlayCardWithPlaylistItem Item;

            private readonly Bindable<MultiplayerPlaylistItem?> playlistItem = new Bindable<MultiplayerPlaylistItem?>();

            private readonly Container shadow;
            private readonly Container content;

            public CardFacade? Facade { get; private set; }

            [Resolved]
            private BeatmapLookupCache beatmapLookupCache { get; set; } = null!;

            public Card(RankedPlayCardWithPlaylistItem item)
            {
                Item = item;

                Size = RankedPlayCard.SIZE;
                Origin = Anchor.Centre;

                InternalChildren = new Drawable[]
                {
                    shadow = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = 25,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Shadow,
                            Radius = 10,
                            Colour = Color4.Black.Opacity(0.1f),
                        }
                    },
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Masking = true,
                        CornerRadius = 25,
                        BorderColour = Color4.Yellow,
                        BorderThickness = 0,
                        Child = new RankedPlayCardBackSide()
                    },
                    new HoverClickSounds()
                };
            }

            private readonly Bindable<SpringParameters> cardMovement = new Bindable<SpringParameters>(MovementStyle.Energetic);

            protected override void LoadComplete()
            {
                base.LoadComplete();

                playlistItem.BindTo(Item.PlaylistItem);
                playlistItem.BindValueChanged(onPlaylistItemChanged, true);

                Selected.BindValueChanged(onSelectedChanged, true);

                position = new Vector2Spring(Position, naturalFrequency: 2.5f, response: 1.2f);
                rotation = new FloatSpring(Rotation, naturalFrequency: 3, response: 2f);
                scale = new Vector2Spring(Scale, naturalFrequency: 3, damping: 0.9f, response: 2f);
                elevation = new FloatSpring { Parameters = MovementStyle.Energetic };

                cardMovement.BindValueChanged(e => position.Parameters = e.NewValue, true);
            }

            private ScheduledDelegate? scheduledFacadeChange;

            public void ChangeFacade(CardFacade facade)
            {
                scheduledFacadeChange?.Cancel();
                scheduledFacadeChange = null;

                if (Facade is { } previous)
                    cardMovement.UnbindFrom(previous.CardMovementBindable);

                Facade = facade;

                cardMovement.BindTo(facade.CardMovementBindable);
            }

            public void ChangeFacade(CardFacade facade, double delay)
            {
                if (delay <= 0)
                {
                    ChangeFacade(facade);
                    return;
                }

                scheduledFacadeChange?.Cancel();
                scheduledFacadeChange = Scheduler.AddDelayed(() => ChangeFacade(facade), delay);
            }

            private void onSelectedChanged(ValueChangedEvent<bool> e)
            {
                content.BorderThickness = e.NewValue ? 12.5f : 0;
            }

            private void onPlaylistItemChanged(ValueChangedEvent<MultiplayerPlaylistItem?> e)
            {
                if (e.NewValue != null)
                    Task.Run(() => loadBeatmapAndRevealCard(e.NewValue.BeatmapID));
                else
                    content.Child = new RankedPlayCardBackSide();
            }

            private async Task loadBeatmapAndRevealCard(int beatmapId)
            {
                var beatmap = await beatmapLookupCache.GetBeatmapAsync(beatmapId).ConfigureAwait(false);

                if (beatmap != null)
                    Schedule(() => revealCard(beatmap));
            }

            private void revealCard(APIBeatmap beatmap)
            {
                content.ScaleTo(new Vector2(0, 1), 150, Easing.In)
                       .Then()
                       .Schedule(() => content.Child = new RankedPlayCard(beatmap))
                       .ScaleTo(new Vector2(1), 250, Easing.OutElasticQuarter);
            }

            private Vector2Spring position = null!;
            private FloatSpring rotation = null!;
            private Vector2Spring scale = null!;
            private FloatSpring elevation = null!;

            protected override void Update()
            {
                base.Update();

                if (Facade == null)
                    return;

                Selected.Value = Facade.Selected;

                var drawQuad = Parent!.ToLocalSpace(Facade.ScreenSpaceDrawQuad);

                var originPosition = RelativeOriginPosition;

                var targetPosition = Vector2.Lerp(
                    Vector2.Lerp(drawQuad.TopLeft, drawQuad.TopRight, originPosition.X),
                    Vector2.Lerp(drawQuad.BottomLeft, drawQuad.BottomRight, originPosition.X),
                    originPosition.Y
                );

                float targetRotation = MathHelper.RadiansToDegrees(new Line(drawQuad.TopLeft, drawQuad.TopRight).Theta);
                float targetScale = Vector2.Distance(drawQuad.TopLeft, drawQuad.BottomLeft) / RankedPlayCard.SIZE.Y;

                Position = position.Update(Time.Elapsed, targetPosition);
                Rotation = rotation.Update(Time.Elapsed, targetRotation);
                Scale = scale.Update(Time.Elapsed, new Vector2(targetScale));

                elevation.Update(Time.Elapsed, Facade.Elevation);

                shadow.Scale = new Vector2(100 / (100 + elevation.Current)) * content.Scale;
                shadow.EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 10,
                    Colour = Color4.Black.Opacity(0.1f),
                    Offset = new Vector2(-elevation.Current * 2.5f, elevation.Current)
                };
            }

            public void PopOutAndExpire(double delay = 0)
            {
                this.Delay(delay)
                    .Schedule(() => Facade = null)
                    .ScaleTo(0, 400, Easing.In)
                    .FadeOut(200)
                    .Expire();

                // bit of extra time before it expires to make sure there is no delay
                LifetimeEnd += 1000;
            }

            #region event handling

            protected override bool OnHover(HoverEvent e) => Facade?.OnCardHover(e) ?? base.OnHover(e);

            protected override void OnHoverLost(HoverLostEvent e) => Facade?.OnCardHoverLost(e);

            protected override bool OnClick(ClickEvent e) => Facade?.OnCardClicked(e) ?? base.OnClick(e);

            protected override bool OnMouseDown(MouseDownEvent e) => Facade?.OnCardMouseDown(e) ?? base.OnMouseDown(e);

            protected override void OnMouseUp(MouseUpEvent e) => Facade?.OnCardMouseUp(e);

            protected override bool OnDragStart(DragStartEvent e) => Facade?.OnCardDragStart(e) ?? base.OnDragStart(e);

            protected override void OnDrag(DragEvent e) => Facade?.OnCardDrag(e);

            protected override void OnDragEnd(DragEndEvent e) => Facade?.OnCardDragEnd(e);

            #endregion
        }
    }
}
