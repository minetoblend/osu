// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Containers;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking
{
    public partial class BeatmapSelectionScreen : OsuScreen
    {
        private OsuScrollContainer scroll = null!;
        private Container<Panel> panelFlow = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                scroll = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = panelFlow = new Container<Panel>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            };

            for (int i = 0; i < 150; i++)
            {
                panelFlow.Add(new Panel
                {
                    Size = new Vector2(300, 70),
                });
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            updateLayout(panelFlow, new Vector2(300, 70));
            panelFlow.FinishTransforms(true);
        }

        private void updateLayout(Container<Panel> container, Vector2 panelSize, int? maxItemsPerRow = null, bool centerVertically = false)
        {
            const float spacing = 20f;
            const float duration = 600;
            const float stagger = 5;

            int numPanelsPerRow = (int)((container.ChildSize.X + spacing) / (panelSize.X + spacing));
            if (numPanelsPerRow > maxItemsPerRow)
                numPanelsPerRow = maxItemsPerRow.Value;

            var children = container.Children.ToArray();

            var position = new Vector2(calculateNextRowStart(0), 0);

            if (centerVertically)
            {
                int numRows = (children.Length + numPanelsPerRow - 1) / numPanelsPerRow;
                float totalHeight = (numRows * (panelSize.Y + spacing)) - spacing;

                position.Y = (ChildSize.Y - totalHeight) / 2;
            }

            double delay = 0;

            for (int i = 0; i < children.Length; i++)
            {
                var panel = children[i];

                if (position.X + panelSize.X > panelFlow.ChildSize.X)
                {
                    position.X = calculateNextRowStart(i);
                    position.Y += panelSize.Y + spacing;
                }

                panel
                    .Delay(delay)
                    .MoveTo(position, duration, Easing.OutExpo)
                    .ResizeTo(panelSize, duration, Easing.OutExpo);

                position.X += panelSize.X + spacing;

                delay += stagger;
            }

            float calculateNextRowStart(int currentIndex)
            {
                int remaining = Math.Min(children.Length - currentIndex, numPanelsPerRow);
                float nextRowWidth = ((panelSize.X + spacing) * remaining - spacing);

                return (panelFlow.ChildSize.X - nextRowWidth) / 2;
            }
        }

        public void SelectionLayout()
        {
        }

        public void HidePanels(int count)
        {
            var exceptions = Random.Shared.GetItems(panelFlow.Children.ToArray(), count);

            var remainingPanelContainer = new Container<Panel> { RelativeSizeAxes = Axes.Both };

            AddInternal(remainingPanelContainer);

            scroll.ScrollbarVisible = false;

            foreach (var panel in panelFlow.Children.ToArray())
            {
                if (!exceptions.Contains(panel))
                {
                    panel.PopOut(Random.Shared.NextSingle() * 500);
                }
                else
                {
                    var position = panel.ScreenSpaceDrawQuad.TopLeft;

                    panelFlow.Remove(panel, false);

                    panel.Position = remainingPanelContainer.ToLocalSpace(position);

                    remainingPanelContainer.Add(panel);
                }
            }

            Scheduler.AddDelayed(() =>
            {
                updateLayout(remainingPanelContainer, new Vector2(300, 70), 3, true);
            }, 600);
        }

        private partial class Panel : CompositeDrawable
        {
            public Panel()
            {
                InternalChild = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 6,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.LightSlateGray,
                    }
                };
            }

            public void PopOut(double delay = 0)
            {
                InternalChild.Delay(delay)
                             .ScaleTo(0, 500, Easing.InCubic)
                             .FadeOut(500, Easing.In);

                this.Delay(delay + 500).Expire();
            }
        }
    }
}
