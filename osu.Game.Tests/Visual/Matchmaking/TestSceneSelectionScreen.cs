// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Utils;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Tests.Visual.Matchmaking
{
    public partial class TestSceneSelectionScreen : OsuTestScene
    {
        [Test]
        public void TestSelectionScreen()
        {
            SelectionScreen screen = null!;

            AddStep("add screen", () => Child = screen = new SelectionScreen());

            AddStep("show grid", () => screen.ShowPanels());
            AddStep("transition to random selection", () => screen.DoTransition());
        }
    }

    public partial class SelectionScreen : CompositeDrawable
{
    private readonly FillFlowContainer<OffsetPanel> panelGrid;
    private readonly BufferedContainer<CarouselPanel> carousel;
    private readonly SpriteText revealText;
    private readonly Container content;
    private readonly Box centerline;

    private readonly Bindable<float> transitionProgress = new Bindable<float>();

    private readonly Triangle triangleLeft;
    private readonly Triangle triangleRight;

    public SelectionScreen()
    {
        RelativeSizeAxes = Axes.Both;

        AddRangeInternal([
            centerline = new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = 4,
                Width = 0,
                Depth = 1,
                Alpha = 0.25f,
                AlwaysPresent = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }
        ]);

        AddInternal(content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Children =
            [
                panelGrid = new FillFlowContainer<OffsetPanel>
                {
                    Width = 800,
                    AutoSizeAxes = Axes.Y,
                    Spacing = new Vector2(10),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                revealText = new OsuSpriteText
                {
                    Text = "Selected beatmap",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                    Font = new FontUsage(size: 24)
                },
                carousel = new BufferedContainer<CarouselPanel>
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                },
                triangleLeft = new Triangle
                {
                    Size = new Vector2(50, 35),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Rotation = 90,
                    Alpha = 0,
                },
                triangleRight = new Triangle
                {
                    Size = new Vector2(50, 35),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Rotation = -90,
                    Alpha = 0,
                }
            ]
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        for (int i = 0; i < 20; i++)
        {
            float hue = Random.Shared.NextSingle();

            var colour = Colour4.FromHSL(hue, 1f, 0.8f);

            panelGrid.Add(new OffsetPanel
            {
                Size = new Vector2(300, 50),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Colour = colour,
                Content = { Alpha = 0 },
            });

            carousel.Add(new CarouselPanel(i)
            {
                Colour = colour
            });
        }
    }

    public void ShowPanels()
    {
        for (var i = 0; i < panelGrid.Children.Count; i++)
        {
            panelGrid.Children[i].Content
                     .FadeOut()
                     .Delay((i / 2) * 30)
                     .MoveToY(50)
                     .MoveToY(0, 400, Easing.OutExpo)
                     .FadeIn(350);
        }
    }

    public void DoTransition()
    {
        const double stagger = 20;
        const double duration = 400;

        for (var i = 0; i < panelGrid.Children.Count; i++)
        {
            int row = (panelGrid.Children.Count - i - 1) / 2;

            double delay = (panelGrid.Children.Count - i) * stagger;

            // if ((row + i) % 2 == 0)
            //     delay += stagger * 2;

            // ReSharper disable once PossibleLossOfFraction
            panelGrid.Children[i].Content.Delay(delay)
                     .MoveToY(DrawHeight, duration, Easing.InCubic)
                     .MoveToX(i % 2 == 0 ? 30 : -30, duration, Easing.In)
                     .FadeOut(duration);
        }

        using (BeginDelayedSequence(350))
        {
            centerline
                .Delay(200)
                .ResizeWidthTo(0.8f, 1500, Easing.OutExpo)
                .FadeTo(0)
                .FadeTo(0.25f, 350);

            this.TransformBindableTo(transitionProgress, 1.01f, 4000, Easing.OutCubic)
                .Then()
                .TransformBindableTo(transitionProgress, 0.998f, 1000, Easing.InOutQuad)
                .Then()
                .TransformBindableTo(transitionProgress, 1.0005f, 500, Easing.InOutQuad)
                .Then()
                .TransformBindableTo(transitionProgress, 1f, 300, Easing.InOutQuad)
                .Then(100)
                .Finally(_ => presentFinalBeatmap());

            carousel
                .TransformTo("BlurSigma", new Vector2(5, 30))
                .TransformTo("BlurSigma", Vector2.Zero, 2000, Easing.OutCubic);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        transitionProgress.BindValueChanged(updateCarousel, true);
    }

    private void updateCarousel(ValueChangedEvent<float> progress)
    {
        float range = carousel.Children.Count * 80;
        float startY = -range / 2;
        float endY = startY + range;

        float offset = progress.NewValue * 4 - 1.5f;

        float position = startY + offset * range;

        for (var i = 0; i < carousel.Children.Count; i++)
        {
            var box = carousel.Children[i];

            float y = position + box.Index * 80;

            while (y > endY)
                y -= range;

            float foo = Math.Abs(y) / DrawHeight;

            if (offset > -1f)
                y = float.Lerp(y, y * float.Clamp((1 - foo), 0, 1), 0.4f);

            box.Y = y;

            carousel.ChangeChildDepth(box, Math.Abs(y));

            box.Scale = new Vector2(1.3f - MathF.Pow(foo, 2) * 0.35f);
        }
    }

    private void presentFinalBeatmap()
    {
        revealText.FadeIn()
                  .MoveToX(50)
                  .MoveToX(-250, 1000, Easing.OutExpo);

        centerline.ResizeHeightTo(60, 600, Easing.OutExpo);
        centerline.FadeTo(0.25f, 600);
        centerline.ResizeWidthTo(1.1f, 600, Easing.OutExpo);

        foreach (var box in carousel.Children)
        {
            if (Precision.AlmostEquals(box.Y, 0))
            {
                box.ScaleTo(1.8f, 1000, Easing.OutElasticHalf)
                   .MoveToX(150, 1000, Easing.OutExpo);

                box.Flash();
                continue;
            }

            box
                .Delay(Math.Abs(box.Y / 70) * 20)
                .MoveToY(box.Y + 30 * MathF.Sign(box.Y), 1000, Easing.OutElasticHalf);
        }
    }

    private partial class CarouselPanel : CompositeDrawable
    {
        public readonly int Index;

        public CarouselPanel(int index)
        {
            Index = index;
            Size = new Vector2(300, 50);
            Scale = new Vector2(1.2f);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Masking = true;
            CornerRadius = 6;
            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Radius = 20,
                Colour = Color4Extensions.Darken(Colour, 0.5f).Opacity(0.3f),
            };
        }

        public void Flash()
        {
            var box = new Box
            {
                Size = new Vector2(1000),
                RelativePositionAxes = Axes.X,
                Alpha = 0,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                Blending = BlendingParameters.Additive,
                Rotation = -10,
                AlwaysPresent = true,
            };

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 6,
                BorderColour = Color4.White.Opacity(0.2f),
                BorderThickness = 3,
                Child = box,
            });

            box.MoveToX(1.3f, 1000, Easing.OutCubic);
            box.FadeTo(0.5f, 50)
               .Then()
               .FadeOut(1000, Easing.Out);
        }
    }

    private partial class OffsetPanel : CompositeDrawable
    {
        public readonly Drawable Content;

        public OffsetPanel()
        {
            InternalChild = Content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 6,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                }
            };
        }
    }
}

}
