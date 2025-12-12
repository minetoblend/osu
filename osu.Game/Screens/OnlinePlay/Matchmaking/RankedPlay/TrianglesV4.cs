// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public class TrianglesV4 : CompositeDrawable
    {
        private Texture triangleTexture = null!;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            triangleTexture = textures.Get("triangle");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            spawnParticles();
        }

        public float ParticleVelocity = 1;

        protected override void Update()
        {
            base.Update();

            if (DrawHeight <= 0)
                return;

            float baseVelocity = 0.03f * ParticleVelocity / DrawHeight;

            foreach (var child in InternalChildren)
            {
                var particle = (Particle)child;

                particle.Y -= baseVelocity * (float)Time.Elapsed * particle.MovementSpeed;
                particle.Rotation += particle.AngularVelocity * (float)Time.Elapsed * 0.02f;

                if (particle.Y < -0.2f)
                {
                    particle.Y = 1.2f;
                    particle.X = RNG.NextSingle();
                    particle.Alpha = 0.5f + RNG.NextSingle() * 0.5f;
                }
                else if (particle.Y > 1.2f)
                {
                    particle.Y = -0.2f;
                    particle.X = RNG.NextSingle();
                    particle.Alpha = 0.5f + RNG.NextSingle() * 0.5f;
                }
            }
        }

        private void spawnParticles()
        {
            while (InternalChildren.Count < 20)
            {
                var triangle = new Particle
                {
                    Texture = triangleTexture,
                    RelativePositionAxes = Axes.Both,
                    X = RNG.NextSingle(),
                    Y = -0.2f + RNG.NextSingle() * 1.4f,
                    Origin = Anchor.Centre,
                    Rotation = RNG.NextSingle() * 360,
                    Size = new Vector2(400 + RNG.NextSingle() * 400),
                    // Blending = BlendingParameters.Additive,
                    MovementSpeed = 0.25f + RNG.NextSingle() * 0.75f,
                    AngularVelocity = RNG.NextSingle() - 0.75f,
                    Alpha = 0.5f + RNG.NextSingle() * 0.5f,
                };

                AddInternal(triangle);
            }
        }

        private partial class Particle : Sprite
        {
            public float MovementSpeed = 1;
            public float AngularVelocity;
        }
    }
}
