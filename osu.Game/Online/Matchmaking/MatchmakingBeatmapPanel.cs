// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Multiplayer;
using osu.Game.Overlays.Notifications;
using osu.Game.Users.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingBeatmapPanel : CompositeDrawable
    {
        private const int panel_width = 300;

        public Action<APIBeatmap>? OnSelectRequested;

        private readonly APIBeatmap beatmap;

        private Drawable background = null!;
        private FillFlowContainer<SelectionBadge> badges = null!;

        public MatchmakingBeatmapPanel(APIBeatmap beatmap)
        {
            this.beatmap = beatmap;
            Size = new Vector2(panel_width, 50);
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.SaddleBrown
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 15,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = colours.ForStarDifficulty(beatmap.StarRating)
                            },
                            new StarRatingDisplay(new StarDifficulty(beatmap.StarRating, beatmap.MaxCombo ?? 0), StarRatingDisplaySize.Small)
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                        }
                    },
                    new TruncatingSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        MaxWidth = panel_width,
                        Text = beatmap.GetDisplayTitleRomanisable()
                    },
                    badges = new AlwaysUpdateFillFlowContainer<SelectionBadge>
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Margin = new MarginPadding(5),
                        AutoSizeAxes = Axes.Both,
                        Spacing = new Vector2(2),
                    }
                }
            };
        }

        public void AddSelection(MultiplayerRoomUser user)
        {
            if (!badges.Any(b => b.User.Equals(user)))
                badges.Add(new SelectionBadge(user));
        }

        public void RemoveSelection(MultiplayerRoomUser user)
        {
            badges.RemoveAll(b => b.User.Equals(user), true);
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeColour(Color4.SaddleBrown.Lighten(0.2f), 200);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeColour(Color4.SaddleBrown, 100);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            background.FlashColour(Color4.SaddleBrown.Lighten(0.5f), 200, Easing.OutQuint);
            OnSelectRequested?.Invoke(beatmap);
            return true;
        }

        private class SelectionBadge : CompositeDrawable
        {
            public readonly MultiplayerRoomUser User;

            public SelectionBadge(MultiplayerRoomUser user)
            {
                User = user;
                Size = new Vector2(10);

                InternalChild = new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = new UpdateableAvatar(user.User)
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                };
            }
        }
    }
}
