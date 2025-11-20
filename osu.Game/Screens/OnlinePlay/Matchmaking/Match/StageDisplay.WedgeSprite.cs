// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match
{
    public partial class StageDisplay
    {
        public partial class WedgeSprite : CompositeDrawable
        {
            private const float corner_radius = 3f;
            private static readonly Vector2 shear = new Vector2(0.75f, 0);

            private readonly BufferedContainer content;

            public WedgeSprite()
            {
                InternalChild = content = new BufferedContainer(cachedFrameBuffer: true)
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Right = 20 },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            X = 0.1f,
                            Size = new Vector2(1),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Height = 0.5f,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.BottomRight,
                            Shear = -shear,
                            Padding = new MarginPadding { Bottom = -1.5f },
                            Child = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Masking = true,
                                CornerRadius = corner_radius,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                }
                            },
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Height = 0.5f,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.TopRight,
                            Padding = new MarginPadding { Top = -1.5f },
                            Shear = shear,
                            Child = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft,
                                Masking = true,
                                CornerRadius = corner_radius,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                }
                            },
                        },
                    }
                };
            }

            protected override void Update()
            {
                base.Update();

                float value = DrawHeight * shear.X / 2;

                content.Padding = new MarginPadding { Left = value };
            }

            public new BlendingParameters Blending
            {
                get => content.EffectBlending;
                set => content.EffectBlending = value;
            }
        }
    }
}
