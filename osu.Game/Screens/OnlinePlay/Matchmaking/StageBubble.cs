// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Matchmaking;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Multiplayer.MatchTypes.Matchmaking;
using osu.Game.Overlays;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking
{
    internal partial class StageBubble : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private readonly MatchmakingRoomStatus status;
        private readonly LocalisableString displayText;
        private Drawable progressBar = null!;
        private OutsideBorder border;

        private DateTimeOffset countdownStartTime;
        private DateTimeOffset countdownEndTime;

        public StageBubble(MatchmakingRoomStatus status, LocalisableString displayText)
        {
            this.status = status;
            this.displayText = displayText;

            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider? colourProvider)
        {
            var colourDark = colourProvider?.Colour4 ?? Color4.Salmon.Darken(0.3f);
            var colourLight = colourProvider?.Colour3 ?? Color4.Salmon;

            InternalChildren = new Drawable[]
            {
                border = new OutsideBorder
                {
                    RelativeSizeAxes = Axes.Both,
                    Shear = OsuGame.SHEAR,
                    CornerRadius = 4,
                    Colour = colourProvider?.Light1 ?? Color4.White,
                    Alpha = 0.5f,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Shear = OsuGame.SHEAR,
                    Masking = true,
                    CornerRadius = 4,
                    Children = new[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourDark
                        },
                        progressBar = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            RelativePositionAxes = Axes.X,
                            Alpha = 0,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = ColourInfo.GradientHorizontal(colourLight, colourLight.Lighten(0.2f)),
                                },
                                new TrianglesV2
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    RelativePositionAxes = Axes.X,
                                    ScaleAdjust = 0.4f,
                                    Alpha = 0.3f,
                                },
                            }
                        },
                        new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = displayText,
                            Padding = new MarginPadding(10),
                            Shear = -OsuGame.SHEAR,
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchRoomStateChanged += onMatchRoomStateChanged;
            client.CountdownStarted += onCountdownStarted;
            client.CountdownStopped += onCountdownStopped;

            if (client.Room != null)
            {
                onMatchRoomStateChanged(client.Room.MatchState);
                foreach (var countdown in client.Room.ActiveCountdowns)
                    onCountdownStarted(countdown);
            }
        }

        protected override void Update()
        {
            base.Update();

            Padding = new MarginPadding { Left = DrawHeight * OsuGame.SHEAR.X };

            TimeSpan duration = countdownEndTime - countdownStartTime;

            if (duration.TotalMilliseconds == 0)
            {
                progressBar.Width = 0;
            }
            else
            {
                TimeSpan elapsed = DateTimeOffset.Now - countdownStartTime;
                float offset = float.Clamp((float)(elapsed.TotalMilliseconds / duration.TotalMilliseconds), 0, 1);
                progressBar.Width = offset;
            }
        }

        private void onMatchRoomStateChanged(MatchRoomState? state) => Scheduler.Add(() =>
        {
            if (state is not MatchmakingRoomState matchmakingState)
                return;

            if (matchmakingState.RoomStatus == MatchmakingRoomStatus.RoundStart)
            {
                countdownStartTime = countdownEndTime = DateTimeOffset.Now;
                activate();
            }
        });

        private void onCountdownStarted(MultiplayerCountdown countdown) => Scheduler.Add(() =>
        {
            if (countdown is not MatchmakingStatusCountdown matchmakingStatusCountdown || matchmakingStatusCountdown.Status != status)
                return;

            countdownStartTime = DateTimeOffset.Now;
            countdownEndTime = countdownStartTime + countdown.TimeRemaining;
            activate();

            progressBar.FadeIn();
            border.TransformBorderThicknessTo(3, 200, Easing.OutExpo);
        });

        private void onCountdownStopped(MultiplayerCountdown countdown) => Scheduler.Add(() =>
        {
            if (countdown is not MatchmakingStatusCountdown matchmakingStatusCountdown || matchmakingStatusCountdown.Status != status)
                return;

            countdownEndTime = DateTimeOffset.Now;
            deactivate();

            progressBar.FadeOut(200);
            border.TransformBorderThicknessTo(0, 200, Easing.OutExpo);
        });

        private void activate()
        {
            this.FadeTo(1, 200);
        }

        private void deactivate()
        {
            this.FadeTo(0.5f, 200);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (client.IsNotNull())
            {
                client.MatchRoomStateChanged -= onMatchRoomStateChanged;
                client.CountdownStarted -= onCountdownStarted;
                client.CountdownStopped -= onCountdownStopped;
            }
        }
    }
}
