// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public class SecondOrderDynamics
    {
        public Vector2 Current;
        public Vector2 Velocity;
        private float k1;
        private float k2;
        private float k3;

        private DynamicsParameters parameters = new DynamicsParameters();

        public DynamicsParameters Parameters
        {
            get => parameters;
            set
            {
                parameters = value;

                k1 = parameters.Damping / (MathF.PI * parameters.Frequency);
                k2 = 1 / (2 * MathF.PI * parameters.Frequency * (2 * MathF.PI * parameters.Frequency));
                k3 = parameters.Response * parameters.Damping / (2 * MathF.PI * parameters.Frequency);
            }
        }

        public SecondOrderDynamics(Vector2 initialValue, DynamicsParameters parameters)
        {
            Parameters = parameters;
            Current = initialValue;
        }

        public Vector2 Update(float dt, Vector2 target)
        {
            float k2Stable = MathF.Max(MathF.Max(k2, (dt * dt) / 2 + (dt * k1) / 2), dt * k1);

            Current += dt * Velocity;
            Velocity += (dt * (target + k3 * Velocity - Current - k1 * Velocity)) / k2Stable;

            return Current;
        }
    }

    public readonly record struct DynamicsParameters(
        float Frequency,
        float Damping,
        float Response
    );
}
