// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input;
using osu.Framework.Input.StateChanges;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.UI.Cursor;
using osu.Game.Rulesets.UI;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class OsuGameplayVisualizer : CompositeDrawable
    {
        [Resolved]
        private EditorBeatmap editorBeatmap { get; set; } = null!;

        [Resolved]
        private Playfield playfield { get; set; } = null!;

        [Resolved]
        private EditorClock editorClock { get; set; } = null!;

        private readonly SecondOrderDynamics cursorPosition =
            new SecondOrderDynamics(new Vector2(256, 192), new DynamicsParameters());

        private CursorContainer cursorContainer;
        private PassThroughInputManager inputManager;

        [BackgroundDependencyLoader]
        private void load()
        {
            Clock = editorClock;
            ProcessCustomClock = false;

            AddInternal(
                inputManager = new PassThroughInputManager
                {
                    UseParentInput = false,
                    Child = new OsuCursorContainer(),
                }
            );
        }

        protected override void Update()
        {
            base.Update();

            updateMovement();

            new MousePositionAbsoluteInput { Position = playfield.ToScreenSpace(cursorPosition.current) }.Apply(inputManager.CurrentState, inputManager);
        }

        private Vector2 sliderPositionAccurate(Slider slider, double time)
        {
            double completionProgress = Math.Clamp((Time.Current - slider.StartTime) / slider.Duration, 0, 1);

            return slider.StackedPositionAt(completionProgress);
        }

        private void updateMovement()
        {
            if (Time.Elapsed < 0)
                return;

            var (current, next) = getCurrentAndNext();

            if (current is Spinner spinner && isActive(spinner))
            {
                float angle = (float)(Time.Current - current.StartTime) * 0.05f;

                int spinCount = (int)(angle / (Math.PI * 2));

                float scaleX = 0.5f + random(new Vector2(spinCount, 0)) * 0.4f;
                float scaleY = 0.8f + random(new Vector2(spinCount, 1)) * 0.6f;

                var position = new Vector2(256, 192) +
                               rotate(
                                   rotate(new Vector2(0, 120), angle) * new Vector2(scaleX, scaleY),
                                   MathF.PI * (0.2f + random(new Vector2(spinCount, 12)) * 0.1f)
                               );

                moveCursor(
                    position,
                    frequency: 4 + Random.Shared.NextSingle(),
                    damping: 0.8f + Random.Shared.NextSingle(),
                    response: 0.5f
                );

                return;
            }

            if (current is Slider slider && isActive(slider))
            {
                var position = sliderPathPositionAt(slider, Time.Current, next);

                float followSliderAccuratelyFactor = float.Clamp(Interpolation.ValueAt(slider.Velocity, 1f, 0f, 0f, 2f), 0, 1);

                followSliderAccuratelyFactor *= float.Clamp(Interpolation.ValueAt(slider.SpanDuration, 0f, 1f, 50f, 150f), 0, 1);

                position = Vector2.Lerp(position, sliderPositionAccurate(slider, Time.Current), followSliderAccuratelyFactor);

                var scale = float.Clamp((position - cursorPosition.current).Length / (float)slider.Radius, 0.5f, 6f);

                moveCursor(
                    position,
                    frequency: Math.Max(scale * 5f, 5f),
                    damping: Math.Max(1f, 2f - scale),
                    response: 0.5f
                );

                moveCursor(
                    sliderPositionAccurate(slider, Time.Current),
                    frequency: 1f,
                    damping: 1f,
                    response: 3f
                );

                return;
            }

            if (current != null && next != null)
            {
                Vector2 prevPosition = getEndPositionWithLeniency(current, next);
                Vector2 nextPosition = moveIntoCircle((prevPosition + next.StackedPosition) / 2, next.StackedPosition,
                    next.Radius * 0.5f);

                var delta = nextPosition - prevPosition;

                double startTime = Math.Max(getLooseEndTime(current), next.StartTime - next.TimePreempt);
                double endTime = next.StartTime;
                double duration = endTime - startTime;

                if (Time.Current < startTime)
                {
                    moveCursor(
                        cursorPosition.current,
                        frequency: 2,
                        damping: 0.5f,
                        response: 0.5f
                    );

                    return;
                }

                var position = Interpolation.ValueAt(
                    Time.Current,
                    prevPosition,
                    nextPosition,
                    startTime,
                    startTime + MathF.Min(200f, (float)duration * 0.75f),
                    Easing.Out
                );

                bool mostlyHorizontal = Math.Abs(delta.X) > Math.Abs(delta.Y);

                var completionProgress =
                    float.Clamp((float)(((float)Time.Current - current.GetEndTime()) / (next.StartTime - current.GetEndTime())), 0, 1);

                float frequencyMultiplier = 1;

                if (delta.Length > 125)
                {
                    var curveFactor = MathF.Pow(1 - completionProgress, 2);

                    if (mostlyHorizontal)
                        position.Y -= curveFactor * delta.Length * 0.1f;
                    else
                        position.Y -= curveFactor * delta.Length * 0.1f;

                    frequencyMultiplier *= 1.5f;
                }

                float radius = (float)next.Radius;

                position += new Vector2(
                    (random(current.StackedPosition) - 0.5f) * radius * 0.25f,
                    (random(current.StackedPosition + new Vector2(1, 0)) - 0.5f) * radius * 0.25f
                );

                float distanceFactor = 1f + MathF.Log(delta.Length + 1) * 0.05f;

                float frequency = (float)(duration > 0
                    ? (1000f / duration) * 0.75f * distanceFactor * frequencyMultiplier
                    : 10);

                float fadeIn = Interpolation.ValueAt(
                    Time.Current,
                    0.15f,
                    1f,
                    startTime,
                    startTime + MathF.Min(250, (float)duration * 0.75f)
                );

                moveCursor(
                    position,
                    frequency: MathF.Min(MathF.Max(frequency, 0.5f) * fadeIn, 2) * 3f,
                    damping: 1,
                    response: 0.75f * fadeIn
                );
            }
            else
            {
                var position = next?.StackedPosition ?? cursorPosition.current;
                moveCursor(
                    position,
                    frequency: 2,
                    damping: 0.5f,
                    response: 0.5f
                );
            }
        }

        private bool isActive(HitObject hitObject)
        {
            return Time.Current >= hitObject.StartTime && Time.Current <= getLooseEndTime(hitObject);
        }

        private Vector2 moveIntoCircle(Vector2 position, Vector2 center, double radius)
        {
            if (Vector2.Distance(position, center) <= radius)
                return position;

            var direction = position - center;
            direction.Normalize();

            return center + direction * (float)radius;
        }

        private Vector2 getEndPositionWithLeniency(OsuHitObject hitObject, OsuHitObject? next)
        {
            double time = getLooseEndTime(hitObject);

            if (hitObject is Slider slider)
            {
                return sliderPathPositionAt(slider, time, next);
            }

            return hitObject.EndPosition;
        }

        private double getLooseEndTime(HitObject hitObject)
        {
            double endTime = hitObject.GetEndTime();

            if (hitObject is Spinner)
                return Math.Max(hitObject.StartTime, hitObject.GetEndTime() - 150);

            if (hitObject is Slider slider)
            {
                if (slider.Duration < 100 && Vector2.Distance(slider.StackedPosition, slider.StackedEndPosition) < 50)
                {
                    return slider.StartTime;
                }

                endTime -= 36;

                endTime = Math.Max(endTime, slider.GetEndTime() - slider.SpanDuration);

                endTime = Math.Max(endTime, slider.NestedHitObjects.LastOrDefault(it => it is SliderTick or SliderRepeat)?.GetEndTime() ?? 0);
            }

            return endTime;
        }

        private Vector2 sliderPathPositionAt(Slider slider, double time, OsuHitObject? nextObject)
        {
            var positions = approximateSliderPath(slider);

            if (time <= slider.StartTime || positions.Count == 0)
                return slider.StackedPosition;

            if (time >= slider.EndTime)
                return slider.EndPosition;

            if (nextObject != null)
            {
                if (positions.Count > 1)
                {
                    var lastPosition = positions[^2].position;
                    var nextPosition = nextObject.StackedPosition;
                    var optimalPosition = Interpolation.ValueAt(
                        Time.Current,
                        lastPosition,
                        nextPosition,
                        positions[^2].time,
                        nextObject.StartTime
                    );

                    positions.Insert(positions.Count - 1, (
                        Vector2.Lerp(positions[^1].position, positions[^2].position, 0.5f),
                        double.Lerp(positions[^1].time, positions[^2].time, 0.5f)
                    ));

                    positions[^1] = positions[^1] with
                    {
                        position = moveIntoCircle(optimalPosition, sliderPositionAccurate(slider, getLooseEndTime(slider)),
                            nextObject.Radius * 1f)
                    };
                }
                else
                {
                    positions[^1] = positions[^1] with
                    {
                        position = moveIntoCircle(nextObject.StackedPosition, slider.StackedEndPosition,
                            nextObject.Radius * 1f)
                    };
                }
            }
            else
            {
                positions[^1] = positions[^1] with { position = slider.EndPosition };
            }

            for (var i = 0; i < positions.Count - 1; i++)
            {
                if (time > positions[i + 1].time)
                    continue;

                var current = positions[i].position;
                var next = positions[i + 1].position;

                return Interpolation.ValueAt(time, current, next, positions[i].time, positions[i + 1].time);
            }

            return slider.EndPosition;
        }

        private List<(Vector2 position, double time)> approximateSliderPath(Slider slider)
        {
            var position = slider.StackedPosition;

            var positions = new List<(Vector2 position, double time)> { (position, slider.StartTime) };

            foreach (var nested in slider.NestedHitObjects)
            {
                if (nested is not SliderTick && nested is not SliderRepeat && nested is not SliderTailCircle)
                    continue;

                double tickTime = nested.GetEndTime();

                var nextPosition = ((OsuHitObject)nested).StackedPosition;

                if (tickTime - positions[^1].time > 300)
                    position = nextPosition;
                else
                {
                    position = moveIntoCircle(
                        position,
                        nextPosition,
                        slider.Radius
                    );
                }

                positions.Add((position, tickTime));
            }

            return positions;
        }

        private Vector2 rotate(Vector2 v, float angle)
        {
            (float sin, float cos) = MathF.SinCos(angle);
            return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
        }

        private float random(Vector2 st)
        {
            var x = MathF.Sin(Vector2.Dot(new Vector2(12.9898f, 78.233f), st)) * 43758.5453123f;

            return x - MathF.Floor(x);
        }

        private (OsuHitObject?, OsuHitObject?) getCurrentAndNext()
        {
            var hitObjects = playfield.HitObjectContainer.AliveObjects.ToList();

            int activeIndex = hitObjects.FindLastIndex(h => editorClock.CurrentTime >= h.HitObject.StartTime);

            return (
                hitObjects.ElementAtOrDefault(activeIndex)?.HitObject as OsuHitObject,
                hitObjects.ElementAtOrDefault(activeIndex + 1)?.HitObject as OsuHitObject
            );
        }

        private void moveCursor(
            Vector2 position,
            float frequency,
            float damping,
            float response
        )
        {
            position = new Vector2(
                float.Clamp(position.X, -512, 1024),
                float.Clamp(position.Y, -384, 768)
            );

            cursorPosition.Parameters = new DynamicsParameters(Frequency: frequency, Damping: damping, Response: response);

            cursorPosition.Update(float.Clamp((float)Time.Elapsed / 1000, 0f, 0.1f), position);
        }

        public class SecondOrderDynamics
        {
            public Vector2 previous;
            public Vector2 current;
            public Vector2 velocity;
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
                    k3 = (parameters.Response * parameters.Damping) / (2 * MathF.PI * parameters.Frequency);
                }
            }

            public SecondOrderDynamics(Vector2 initialValue, DynamicsParameters parameters)
            {
                Parameters = parameters;
                previous = initialValue;
                current = initialValue;
            }

            public Vector2 Update(float dt, Vector2 target)
            {
                float k2Stable = MathF.Max(MathF.Max(k2, (dt * dt) / 2 + (dt * k1) / 2), dt * k1);

                current += dt * velocity;
                velocity += (dt * (target + k3 * velocity - current - k1 * velocity)) / k2Stable;

                return current;
            }
        }

        public readonly record struct DynamicsParameters(
            float Frequency,
            float Damping,
            float Response
        );
    }
}
