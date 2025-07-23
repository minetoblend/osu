// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.Multiplayer;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Online.Matchmaking
{
    public class MatchmakingPlayerPanel : CompositeDrawable
    {
        public readonly MultiplayerRoomUser User;

        private OsuSpriteText rankText = null!;
        private OsuSpriteText scoreText = null!;
        private int rank;
        private int score;

        public MatchmakingPlayerPanel(MultiplayerRoomUser user)
        {
            User = user;
            Size = new Vector2(200, 50);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.SaddleBrown
                    },
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding
                        {
                            Horizontal = 5
                        },
                        ColumnDimensions =
                        [
                            new Dimension(GridSizeMode.AutoSize),
                            new Dimension(),
                            new Dimension(GridSizeMode.AutoSize)
                        ],
                        Content = new Drawable[][]
                        {
                            [
                                rankText = new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding { Left = 5 },
                                    Text = User.User!.Username
                                },
                                scoreText = new OsuSpriteText
                                {
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                }
                            ]
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Rank = 0;
            Score = 0;
        }

        public int Rank
        {
            get => rank;
            set
            {
                rank = value;
                rankText.Text = value == 0 ? "--" : $"#{value}";
            }
        }

        public int Score
        {
            get => score;
            set
            {
                score = value;
                scoreText.Text = $"{value}pts";
            }
        }
    }
}
