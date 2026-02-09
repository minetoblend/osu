// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Components
{
    public partial class ScoreCounter : CompositeDrawable
    {
        private readonly FillFlowContainer digitFlow;
        private readonly CounterDigit[] digits;

        public required FontUsage Font { get; init; }

        private long value;

        public long Value
        {
            get => value;
            set
            {
                this.value = value;
                if (!IsLoaded)
                    return;

                updateDigits();
            }
        }

        public TransformSequence<ScoreCounter> TransformValueTo(long value, double duration = 0, Easing easing = Easing.None) => this.TransformTo(nameof(Value), value, duration, easing);

        public ScoreCounter(int numDigits = 6)
        {
            digits = new CounterDigit[numDigits];

            AutoSizeAxes = Axes.Both;

            InternalChildren =
            [
                digitFlow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                }
            ];
        }

        public Vector2 Spacing
        {
            get => digitFlow.Spacing;
            set => digitFlow.Spacing = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            for (int i = 0; i < digits.Length; i++)
            {
                if ((digits.Length - i) % 3 == 0 && i != 0)
                {
                    digitFlow.Add(new OsuSpriteText
                    {
                        Text = ",",
                        Font = Font,
                    });
                }

                digits[i] = new CounterDigit
                {
                    Font = Font.With(fixedWidth: true),
                };

                digitFlow.Add(digits[i]);
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            updateDigits();
        }

        public void SetValueInstantly(long value)
        {
            this.value = value;
            updateDigits(false);
        }

        private void updateDigits(bool animated = true)
        {
            long current = value;

            for (int i = digits.Length - 1; i >= 0; i--)
            {
                digits[i].Offset = current;

                if (!animated)
                    digits[i].CompleteAnimations();

                current /= 10;
            }
        }

        private partial class CounterDigit : CompositeDrawable
        {
            private readonly DoubleSpring spring = new DoubleSpring
            {
                Damping = 0.8f,
                NaturalFrequency = 2.5f,
                Response = 1f
            };

            public double Offset { get; set; }

            private OsuSpriteText lower = null!;
            private OsuSpriteText upper = null!;

            private BufferedContainer blurContainer = null!;

            public required FontUsage Font { get; init; }

            [BackgroundDependencyLoader]
            private void load()
            {
                Debug.Assert(Font.FixedWidth);

                InternalChild = blurContainer = new BufferedContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = 3f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    BackgroundColour = Colour4.White.Opacity(0),
                    Children =
                    [
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Height = 1f / 3f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children =
                            [
                                upper = new OsuSpriteText
                                {
                                    Text = "9",
                                    Font = Font,
                                    RelativePositionAxes = Axes.Y,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                },
                                lower = new OsuSpriteText
                                {
                                    Text = "0",
                                    Font = Font,
                                    RelativePositionAxes = Axes.Y,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                }
                            ]
                        }
                    ]
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                Size = lower.DrawSize;
            }

            protected override void Update()
            {
                base.Update();

                spring.Damping = spring.Velocity > 30 ? 1f : 0.8f;

                spring.Update(Time.Elapsed, Offset);

                updateState();
            }

            private void updateState()
            {
                int digit = (int)spring.Current % 10;
                if (digit < 0) digit += 10;

                lower.Text = digit.ToString();
                upper.Text = ((digit + 1) % 10).ToString();

                float y = (float)(spring.Current % 1);

                if (y < 0)
                    y = 0;

                upper.Y = (y - 1) * 0.65f;
                lower.Y = y * 0.65f;

                lower.Alpha = MathF.Pow(1 - y, 2);
                upper.Alpha = MathF.Pow(y, 2);

                upper.Scale = new Vector2(float.Lerp(0.5f, 1f, MathF.Sqrt(0.5f + y * 0.5f)));
                lower.Scale = new Vector2(float.Lerp(0.5f, 1f, MathF.Sqrt(1 - y * 0.5f)));

                blurContainer.BlurSigma = new Vector2(0, float.Clamp((float)Math.Abs(spring.Velocity * 0.1f) - 5, 0, 10));
            }

            public void CompleteAnimations()
            {
                spring.Current = Offset;
                spring.PreviousTarget = Offset;
                spring.Velocity = 0;

                updateState();
            }
        }
    }

    public static class ScoreCounterExtensions
    {
        public static TransformSequence<ScoreCounter> TransformValueTo(this TransformSequence<ScoreCounter> t, long value, double duration = 0, Easing easing = Easing.None) =>
            t.Append(o => o.TransformValueTo(value, duration, easing));
    }
}
