// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps.Drawables.Cards;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class SplitPanel : Container
    {
        protected override Container<Drawable> Content { get; }

        public readonly Container BackgroundLayer;

        public readonly Box Background;

        private readonly Container paddingContainer;

        public SplitPanel()
        {
            InternalChildren = new[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(1),
                    Child = BackgroundLayer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = BeatmapCard.CORNER_RADIUS,
                        CornerExponent = 10,
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Glow,
                            Radius = 4,
                            Colour = Color4.White.Opacity(0.1f)
                        },
                        Child = Background = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                },
                paddingContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Bottom = 3 },
                    Child = Content = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        CornerRadius = BeatmapCard.CORNER_RADIUS,
                        CornerExponent = 10,
                    },
                }
            };
        }

        public float ContentPadding
        {
            get => paddingContainer.Padding.Bottom;
            set => paddingContainer.Padding = new MarginPadding { Bottom = value };
        }
    }
}
