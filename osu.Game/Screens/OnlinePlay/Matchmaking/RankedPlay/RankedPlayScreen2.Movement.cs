// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Utils;
using osu.Game.Utils;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayScreen2
    {
        public static class MovementStyle
        {
            public static SpringParameters Energetic => new SpringParameters
            {
                NaturalFrequency = 3f,
                Damping = 0.9f,
                Response = 1.2f,
            };

            public static SpringParameters Smooth => new SpringParameters
            {
                NaturalFrequency = 2f,
                Damping = 1f,
                Response = 1.2f,
            };
        }

        public class MovementStyleTransform : Transform<SpringParameters, CardFacade>
        {
            public override string TargetMember => nameof(CardFacade.CardMovement);

            protected override void Apply(CardFacade d, double time)
            {
                if (time <= StartTime)
                {
                    d.CardMovement.Value = StartValue;
                    return;
                }

                if (time >= EndTime)
                {
                    d.CardMovement.Value = EndValue;
                    return;
                }

                d.CardMovement.Value = new SpringParameters
                {
                    NaturalFrequency = Interpolation.ValueAt(time, StartValue.NaturalFrequency, EndValue.NaturalFrequency, StartTime, EndTime),
                    Damping = Interpolation.ValueAt(time, StartValue.Damping, EndValue.Damping, StartTime, EndTime),
                    Response = Interpolation.ValueAt(time, StartValue.Response, EndValue.Response, StartTime, EndTime),
                };
            }

            protected override void ReadIntoStartValue(CardFacade d) => StartValue = d.CardMovement.Value;
        }
    }

    public static class RankedPlayScreen2Extensions
    {
        public static TransformSequence<RankedPlayScreen2.CardFacade> TransformMovementStyleTo(
            this RankedPlayScreen2.CardFacade facade,
            SpringParameters value,
            double duration = 0,
            Easing easing = Easing.None
        )
        {
            var transform = facade.PopulateTransform(new RankedPlayScreen2.MovementStyleTransform(), value, duration, easing);

            return facade.TransformTo(transform);
        }

        public static TransformSequence<RankedPlayScreen2.CardFacade> TransformMovementStyleTo(
            this TransformSequence<RankedPlayScreen2.CardFacade> sequence,
            SpringParameters value,
            double duration = 0,
            Easing easing = Easing.None
        ) => sequence.Append(o => o.TransformMovementStyleTo(value, duration, easing));
    }
}
