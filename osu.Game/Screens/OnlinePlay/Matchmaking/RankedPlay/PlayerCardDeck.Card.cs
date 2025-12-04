// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardDeck
    {
        public partial class PlayerCard : CompositeDrawable
        {
            public SpringParameters Movement
            {
                get => spring.Parameters;
                set => spring.Parameters = value;
            }

            public PlayerCard(RankedPlayCard item)
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Size = new Vector2(150, 250) + new Vector2(20);
                Padding = new MarginPadding(10);

                InternalChild = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 20,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(0.1f),
                    },
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.LightGray,
                    }
                };
            }

            public Vector2 TargetPosition;
            public float TargetRotation;

            private Vector2 velocity;
            private float rotationVelocity;

            private Spring spring = new Spring
            {
                NaturalFrequency = 4,
                Damping = 2,
                Response = 1.4f
            };

            protected override void Update()
            {
                base.Update();

                Position = spring.Update(Time.Elapsed, current: Position, target: TargetPosition, velocity: ref velocity);
                Rotation = spring.Update(Time.Elapsed, current: Rotation, target: TargetRotation, velocity: ref rotationVelocity);
            }

            protected override bool OnHover(HoverEvent e)
            {
                this.ScaleTo(1.1f, 400, Easing.OutExpo);

                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                this.ScaleTo(1, 400, Easing.OutExpo);
            }
        }
    }
}
