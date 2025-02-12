// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class ScaleGizmo : CircularContainer
    {
        private const float ring_radius = 14;

        public event Action? OperationBegan = null;

        public event Action? OperationEnded = null;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        private AxisScaleHandle scaleHandleX = null!;
        private AxisScaleHandle scaleHandleY = null!;

        private Box lineX = null!;
        private Box lineY = null!;

        private Box globalLineX = null!;
        private Box globalLineY = null!;

        private Ring innerRing = null!;
        private Ring outerRing = null!;

        private Drawable axisGhost = null!;
        private Drawable uniformGhost = null!;
        private Container axisContainer = null!;

        private DashedLine lineToCursor = null!;

        private readonly BindableWithCurrent<Vector2> current = new BindableWithCurrent<Vector2>(new Vector2(1));

        public Bindable<Vector2> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Origin = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                lineToCursor = new DashedLine
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.CentreLeft,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(6),
                    Child = outerRing = new Ring
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                },
                globalLineX = new Box
                {
                    Colour = colours.Red0,
                    Height = 1.5f,
                    Width = 10_000f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                },
                globalLineY = new Box
                {
                    Colour = colours.Green0,
                    Width = 1.5f,
                    Height = 10_000f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                },
                axisContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(20),
                    Children = new Drawable[]
                    {
                        lineX = new Box
                        {
                            Colour = colours.Red2,
                            Height = 2f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.CentreLeft,
                        },
                        lineY = new Box
                        {
                            Colour = colours.Green2,
                            Width = 2f,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.BottomCentre,
                        },
                        scaleHandleX = new AxisScaleHandle(this, Axes.X)
                        {
                            InactiveColour = colours.Red2,
                            ActiveColour = colours.Red1,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        scaleHandleY = new AxisScaleHandle(this, Axes.Y)
                        {
                            InactiveColour = colours.Green2,
                            ActiveColour = colours.Green1,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                        },
                        axisGhost = new CircularContainer
                        {
                            Masking = true,
                            Size = new Vector2(14),
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Alpha = 0,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.1f,
                            }
                        },
                    }
                },
                new Container
                {
                    Size = new Vector2(ring_radius * 2),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        uniformGhost = new Ring
                        {
                            Alpha = 0,
                        },
                        innerRing = new Ring(),
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            current.BindValueChanged(_ => Scheduler.AddOnce(updateLayout), true);

            updateColour();
        }

        private Axes draggedAxes;
        private float dragStartDistance;

        protected override bool OnHover(HoverEvent e)
        {
            updateColour();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            updateColour();
        }

        private void updateColour()
        {
            innerRing.Colour = outerRing.Colour = IsHovered | IsDragged ? Color4.White : Color4Extensions.FromHex("CCCCCC");
        }

        private void beginScaleOperation(Axes axes)
        {
            draggedAxes = axes;

            updateLayout();

            lineToCursor.Show();
            outerRing.Hide();

            switch (axes)
            {
                case Axes.Both:
                    uniformGhost.Alpha = 0.1f;
                    axisContainer.Hide();
                    break;

                case Axes.X:
                    lineY.Hide();
                    scaleHandleY.Hide();
                    innerRing.Hide();

                    globalLineX.Show();

                    axisGhost.Show();
                    axisGhost.Anchor = Anchor.CentreRight;
                    uniformGhost.Hide();
                    break;

                case Axes.Y:
                    lineX.Hide();
                    scaleHandleX.Hide();
                    innerRing.Hide();

                    globalLineY.Show();

                    axisGhost.Show();
                    axisGhost.Anchor = Anchor.TopCentre;
                    uniformGhost.Hide();
                    break;
            }

            OperationBegan?.Invoke();
        }

        private void updateScaleFromEvent(Axes axes, DragEvent e)
        {
            var position = ToLocalSpace(e.ScreenSpaceMousePosition) - DrawSize / 2;

            float direction = MathF.Atan2(position.Y, position.X) * 180f / MathF.PI;

            lineToCursor.Rotation = direction;
            lineToCursor.Width = position.Length;

            var referenceSize = axisContainer.ChildSize;

            var scale = Vector2.One;

            switch (axes)
            {
                case Axes.X:
                    scale.X = position.X / referenceSize.X * 2;
                    break;

                case Axes.Y:
                    scale.Y = position.Y / referenceSize.Y * 2;
                    break;

                case Axes.Both:
                    scale = new Vector2(position.Length / dragStartDistance);
                    break;
            }

            Current.Value = scale;
        }

        private void endScaleOperation()
        {
            draggedAxes = Axes.None;

            axisContainer.Show();
            innerRing.Show();
            outerRing.Show();
            lineX.Show();
            lineY.Show();
            scaleHandleX.Show();
            scaleHandleY.Show();

            globalLineX.Hide();
            globalLineY.Hide();

            axisGhost.Hide();
            lineToCursor.Hide();

            updateLayout();

            OperationEnded?.Invoke();
            Current.Value = Vector2.One;
        }

        private void updateLayout()
        {
            outerRing.Size = Current.Value;

            var handleScale = Current.Value * axisContainer.ChildSize / 2;

            scaleHandleX.X = handleScale.X;
            scaleHandleY.Y = -handleScale.Y;

            if (draggedAxes != Axes.None)
            {
                lineX.Width = handleScale.X;
                lineX.X = 0;

                lineY.Height = handleScale.Y;
                lineY.Y = 0;
            }
            else
            {
                lineX.Width = (MathF.Abs(handleScale.X) - ring_radius) * MathF.Sign(handleScale.X);
                lineX.X = handleScale.X > 0 ? ring_radius : -ring_radius;

                lineY.Height = (MathF.Abs(handleScale.Y) - ring_radius) * MathF.Sign(handleScale.Y);
                lineY.Y = handleScale.Y > 0 ? -ring_radius : ring_radius;
            }
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            beginScaleOperation(Axes.Both);

            dragStartDistance = Math.Max(5f, (ToLocalSpace(e.ScreenSpaceMousePosition) - DrawSize / 2).Length);

            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            base.OnDrag(e);

            updateScaleFromEvent(Axes.Both, e);
        }

        protected override void OnDragEnd(DragEndEvent e)
        {
            endScaleOperation();
        }

        private partial class AxisScaleHandle : Box
        {
            public required Color4 InactiveColour { get; init; }
            public required Color4 ActiveColour { get; init; }

            private readonly Axes axis;
            private readonly ScaleGizmo gizmo;

            public AxisScaleHandle(ScaleGizmo gizmo, Axes axis)
            {
                this.gizmo = gizmo;
                this.axis = axis;

                Size = new Vector2(12);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                updateColour();
            }

            protected override bool OnHover(HoverEvent e)
            {
                updateColour();
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                updateColour();
            }

            private void updateColour() => Colour = IsHovered || IsDragged ? ActiveColour : InactiveColour;

            protected override bool OnDragStart(DragStartEvent e)
            {
                updateColour();
                gizmo.beginScaleOperation(axis);
                return true;
            }

            protected override void OnDrag(DragEvent e)
            {
                base.OnDrag(e);

                gizmo.updateScaleFromEvent(axis, e);
            }

            protected override void OnDragEnd(DragEndEvent e)
            {
                base.OnDragEnd(e);

                updateColour();

                gizmo.endScaleOperation();
            }
        }

        private partial class Ring : CircularContainer
        {
            public Ring()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                RelativeSizeAxes = Axes.Both;

                Masking = true;
                BorderThickness = 2;
                BorderColour = Color4.White;

                Child = new Box
                {
                    AlwaysPresent = true,
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both
                };
            }
        }

        private partial class DashedLine : CompositeDrawable
        {
            private const float dash_length = 3f;

            public DashedLine()
            {
                Height = 1;
                Masking = true;
            }

            public override float Width
            {
                get => base.Width;
                set
                {
                    if (base.Width == value)
                        return;

                    base.Width = value;
                    Scheduler.AddOnce(updateDashes);
                }
            }

            private void updateDashes()
            {
                int numDashes = Math.Max(0, (int)(Width / (dash_length * 2)) + 1);

                while (InternalChildren.Count > numDashes)
                    RemoveInternal(InternalChildren[^1], true);

                while (InternalChildren.Count < numDashes)
                {
                    AddInternal(new Box
                    {
                        X = InternalChildren.Count * (dash_length * 2),
                        Width = dash_length,
                        Height = 1,
                        Alpha = 0.3f,
                    });
                }
            }
        }
    }
}
