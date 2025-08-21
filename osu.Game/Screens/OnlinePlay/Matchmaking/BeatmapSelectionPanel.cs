// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osuTK;
using osuTK.Input;

namespace osu.Game.Screens.OnlinePlay.Matchmaking
{
    public partial class BeatmapSelectionPanel : Container
    {
        public static readonly Vector2 SIZE = new Vector2(300, 70);

        private const float corner_radius = 6;
        private const float border_width = 3;

        public Action? Clicked;

        public readonly MultiplayerPlaylistItem Item;

        protected override Container<Drawable> Content { get; }

        private readonly Container scaleContainer;
        private readonly BeatmapSelectionOverlay selectionOverlay;
        private readonly Box flash;
        private readonly Container border;

        public bool AllowSelection = true;

        public BeatmapSelectionPanel(MultiplayerPlaylistItem item)
        {
            Item = item;
            Size = SIZE;
            InternalChild = scaleContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(-border_width),
                        Child = border = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = corner_radius + border_width,
                            Alpha = 0,
                            Child = new Box { RelativeSizeAxes = Axes.Both },
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = corner_radius,
                        Children = new Drawable[]
                        {
                            Content = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                            },
                            flash = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0,
                            },
                        }
                    },
                    selectionOverlay = new BeatmapSelectionOverlay
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding { Horizontal = 10 },
                        Origin = Anchor.CentreLeft,
                    },
                    new HoverClickSounds()
                }
            };
        }

        public bool AddUser(APIUser user, bool isOwnUser) => selectionOverlay.AddUser(user, isOwnUser);

        public bool RemoveUser(int userId) => selectionOverlay.RemoveUser(userId);

        protected override bool OnHover(HoverEvent e)
        {
            if (!AllowSelection)
                return false;

            flash.FadeTo(0.3f, 50)
                 .Then()
                 .FadeTo(0.15f, 300);

            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            flash.FadeOut(200);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (!AllowSelection)
                return false;

            if (e.Button == MouseButton.Left)
            {
                scaleContainer.ScaleTo(0.9f, 400, Easing.OutExpo);
                return true;
            }

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Left)
                scaleContainer.ScaleTo(1f, 500, Easing.OutElasticHalf);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!AllowSelection)
                return false;

            Clicked?.Invoke();

            flash.FadeTo(0.6f, 50)
                 .Then()
                 .FadeTo(0.15f, 400);

            return true;
        }

        public void ShowBorder() => border.Show();

        public void HideBorder() => border.Hide();

        public void PopOutAndExpire(double delay = 0, Easing easing = Easing.InCubic)
        {
            AllowSelection = false;

            const double duration = 400;

            scaleContainer.Delay(delay)
                          .ScaleTo(0, duration, easing)
                          .FadeOut(duration);

            this.Delay(delay + duration).FadeOut().Expire();
        }
    }
}
