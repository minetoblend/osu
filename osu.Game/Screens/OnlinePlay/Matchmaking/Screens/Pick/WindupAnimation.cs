// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Overlays;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Screens.Pick
{
    public partial class WindupAnimation : CompositeDrawable
    {
        public double Duration { get; init; } = 1000;

        [Resolved]
        private OverlayColourProvider? colourProvider { get; set; }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            var rng = new Random();

            for (int i = 0; i < 15; i++)
            {
                float angle = rng.NextSingle() * MathF.PI * 2;
                var direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

                var endPosition = direction * DrawSize / 2;

                var position = endPosition + direction * (300 + rng.NextSingle() * 300);

                var particle = new Triangle
                {
                    Size = new Vector2(15),
                    Position = position,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Rotation = rng.Next(360),
                    Colour = colourProvider?.Light2 ?? Color4.White,
                };

                AddInternal(particle);

                double delay = Random.Shared.NextDouble() * 200;
                double duration = Duration - delay;

                particle.FadeOut()
                        .ScaleTo(0)
                        .Delay(delay)
                        .FadeTo(0.5f + rng.NextSingle() * 0.5f, duration)
                        .RotateTo(particle.Rotation + rng.NextSingle() * 360 - 180, duration, Easing.InCubic)
                        .MoveTo(endPosition, duration, Easing.InCubic)
                        .ScaleTo(0.5f + rng.NextSingle(), duration, Easing.InCubic);
            }

            this.Delay(Duration)
                .FadeOut()
                .Expire();
        }
    }
}
