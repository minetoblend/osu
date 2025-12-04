// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK;

namespace osu.Game.Utils
{
    public struct Spring
    {
        private float k1;
        private float k2;
        private float k3;

        private SpringParameters parameters;

        public SpringParameters Parameters
        {
            get => parameters;
            set
            {
                parameters = value;

                k1 = Damping / (MathF.PI * NaturalFrequency);
                k2 = 1 / ((2 * MathF.PI * NaturalFrequency) * (2 * MathF.PI * NaturalFrequency));
                k3 = Response * Damping / (2 * MathF.PI * NaturalFrequency);
            }
        }

        public float NaturalFrequency
        {
            get => Parameters.NaturalFrequency;
            set => Parameters = Parameters with { NaturalFrequency = value };
        }

        public float Damping
        {
            get => Parameters.Damping;
            set => Parameters = Parameters with { Damping = value };
        }

        public float Response
        {
            get => Parameters.Response;
            set => Parameters = Parameters with { Response = value };
        }

        public Spring(float frequency, float damping, float response)
        {
            Parameters = new SpringParameters(frequency, damping, response);
        }

        public readonly float Update(double elapsed, float current, float target, ref float velocity)
        {
            float dt = (float)(elapsed / 1000);

            float k2Stable = MathF.Max(MathF.Max(k2, dt * dt / 2 + dt * k1 / 2), dt * k1);

            current += dt * velocity;
            velocity += (dt * (target + k3 * velocity - current - k1 * velocity)) / k2Stable;

            return current;
        }

        public readonly Vector2 Update(double elapsed, Vector2 current, Vector2 target, ref Vector2 velocity)
        {
            float dt = (float)(elapsed / 1000);

            float k2Stable = MathF.Max(MathF.Max(k2, dt * dt / 2 + dt * k1 / 2), dt * k1);

            current += dt * velocity;
            velocity += (dt * (target + k3 * velocity - current - k1 * velocity)) / k2Stable;

            return current;
        }
    }

    public readonly record struct SpringParameters(
        float NaturalFrequency = 1,
        float Damping = 1,
        float Response = 1
    );
}
