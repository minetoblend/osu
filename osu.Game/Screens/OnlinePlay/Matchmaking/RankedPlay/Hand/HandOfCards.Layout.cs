// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Hand
{
    public abstract partial class HandOfCards
    {
        public readonly record struct CardLayout
        {
            public required Vector2 Position { get; init; }
            public required float Rotation { get; init; }
            public required float Scale { get; init; }
        }

        public class CardLayoutTransform : Transform<CardLayout, HandCard>
        {
            public override string TargetMember => nameof(HandCard.LayoutTarget);

            private CardLayout valueAt(double time)
            {
                if (time < StartTime)
                    return StartValue;

                if (time >= EndTime)
                    return EndValue;

                return new CardLayout
                {
                    Position = Interpolation.ValueAt(time, StartValue.Position, EndValue.Position, StartTime, EndTime, Easing),
                    Rotation = Interpolation.ValueAt(time, StartValue.Rotation, EndValue.Rotation, StartTime, EndTime, Easing),
                    Scale = Interpolation.ValueAt(time, StartValue.Scale, EndValue.Scale, StartTime, EndTime, Easing),
                };
            }

            protected override void Apply(HandCard d, double time)
            {
                d.LayoutTarget = valueAt(time);
            }

            protected override void ReadIntoStartValue(HandCard d)
            {
                StartValue = d.LayoutTarget;
            }
        }
    }

    public static class CardExtensions
    {
        public static TransformSequence<T> TransformLayoutTo<T>(this TransformSequence<T> sequence, HandOfCards.CardLayout layout, double duration = 0, Easing easing = Easing.None)
            where T : HandOfCards.HandCard =>
            sequence.Append(o => o.TransformLayoutTo(layout, duration, easing));

        public static TransformSequence<T> TransformMovementSpeedTo<T>(this TransformSequence<T> sequence, float value, double duration = 0, Easing easing = Easing.None)
            where T : HandOfCards.HandCard =>
            sequence.Append(o => o.TransformMovementSpeedTo(value, duration, easing));
    }
}
