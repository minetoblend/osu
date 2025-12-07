// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
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
            public static readonly Vector2 SIZE = new Vector2(120, 200);

            public readonly Bindable<bool> AllowSelection = new Bindable<bool>();
            public readonly BindableBool Selected = new BindableBool();

            public readonly RankedPlayCardWithPlaylistItem Item;

            private readonly Bindable<MultiplayerPlaylistItem?> playlistItem = new Bindable<MultiplayerPlaylistItem?>();

            private readonly Box background;
            private readonly OsuSpriteText beatmapIdText;

            public CardFacade? Facade { get; private set; }

            public Card(RankedPlayCardWithPlaylistItem item)
            {
                Item = item;

                Size = SIZE;
                Masking = true;
                CornerRadius = 10;
                BorderColour = Color4.Yellow;
                BorderThickness = 0;
                Origin = Anchor.Centre;
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Radius = 10,
                    Colour = Color4.Black.Opacity(0.1f),
                };

                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.DimGray
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            new OsuSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = $"ID: {item.Card.ID.GetHashCode()}"
                            },
                            beatmapIdText = new OsuSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Hidden"
                            }
                        }
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
                BorderThickness = e.NewValue ? 5 : 0;
            }

            private void onPlaylistItemChanged(ValueChangedEvent<MultiplayerPlaylistItem?> e)
            {
                if (e.NewValue != null)
                {
                    background.Colour = Color4.SlateGray;
                    beatmapIdText.Text = $"Beatmap: {e.NewValue.BeatmapID}";
                }
            }

            private Vector2Spring position = null!;
            private FloatSpring rotation = null!;
            private Vector2Spring scale = null!;

            protected override void Update()
            {
                base.Update();

                if (Facade == null)
                    return;

                Selected.Value = Facade.Selected;

                var drawQuad = Parent!.ToLocalSpace(Facade.ScreenSpaceDrawQuad);

                float targetRotation = MathHelper.RadiansToDegrees(new Line(drawQuad.TopLeft, drawQuad.TopRight).Theta);
                float targetScale = Vector2.Distance(drawQuad.TopLeft, drawQuad.BottomLeft) / SIZE.Y;

                Position = position.Update(Time.Elapsed, drawQuad.Centre);
                Rotation = rotation.Update(Time.Elapsed, targetRotation);
                Scale = scale.Update(Time.Elapsed, new Vector2(targetScale));
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
