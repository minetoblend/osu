// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;
using osuTK.Graphics;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingQueueBanner : CompositeDrawable
    {
        [Resolved]
        private MultiplayerClient client { get; set; } = null!;

        private SpriteText statusText = null!;
        private Drawable background = null!;
        private MatchmakingQueueStatus? currentStatus;

        public MatchmakingQueueBanner()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Children = new[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Yellow
                    },
                    statusText = new OsuSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Margin = new MarginPadding(10),
                        Colour = Color4.Black,
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            client.MatchmakingQueueStatusChanged += onMatchmakingQueueStatusChanged;
            onMatchmakingQueueStatusChanged(null);
        }

        private void onMatchmakingQueueStatusChanged(MatchmakingQueueStatus? status) => Scheduler.Add(() =>
        {
            currentStatus = status;

            switch (status)
            {
                case null:
                    background.Colour = Color4.Yellow;
                    statusText.Text = string.Empty;
                    break;

                case MatchmakingQueueStatus.InQueue inQueue:
                    background.Colour = Color4.Yellow;
                    statusText.Text = $"finding a match ({inQueue.PlayerCount} / {inQueue.RoomSize})...";
                    break;

                case MatchmakingQueueStatus.FoundMatch:
                    background.Colour = Color4.LightBlue;
                    statusText.Text = "match ready! click to join!";
                    break;
            }
        });

        protected override bool OnClick(ClickEvent e)
        {
            if (currentStatus is MatchmakingQueueStatus.FoundMatch found)
            {
                client.JoinRoom(new Room { RoomID = found.RoomId }).FireAndForget();
                background.FlashColour(Color4.LightBlue.Lighten(0.5f), 100, Easing.OutQuint);
                return true;
            }

            return false;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (client.IsNotNull())
                client.MatchmakingQueueStatusChanged -= onMatchmakingQueueStatusChanged;
        }
    }
}
