// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.Containers;
using osuTK;

namespace osu.Game.Rulesets.Edit.UI
{
    public partial class ComposeToolbox : CompositeDrawable
    {
        private readonly Container tabContent;
        private readonly FillFlowContainer<SidebarCategoryButton> buttonFlow;

        private readonly HashSet<SidebarCategory> categories = new HashSet<SidebarCategory>();
        private readonly Dictionary<SidebarCategory, SidebarTabContent> categoryContentMap = new Dictionary<SidebarCategory, SidebarTabContent>();

        private readonly Bindable<SidebarCategory?> activeCategory = new Bindable<SidebarCategory?>();
        private readonly Bindable<bool> expanded = new Bindable<bool>();

        private Sample? samplePopIn, samplePopOut;

        public ComposeToolbox()
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;

            InternalChild = new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Masking = true,
                        AutoSizeDuration = 300,
                        AutoSizeEasing = Easing.OutExpo,
                        Child = tabContent = new Container
                        {
                            Width = 250,
                            RelativeSizeAxes = Axes.Y,
                        },
                    },
                    buttonFlow = new FillFlowContainer<SidebarCategoryButton>
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Direction = FillDirection.Vertical,
                    },
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            samplePopIn = audio.Samples.Get("UI/overlay-pop-in");
            samplePopOut = audio.Samples.Get("UI/overlay-pop-out");
        }

        public void Add(SidebarCategory category, EditorToolboxGroup toolboxGroup)
        {
            ensureCategoryAdded(category);

            categoryContentMap[category].Add(toolboxGroup);
        }

        public void AddRange(SidebarCategory category, IEnumerable<EditorToolboxGroup> panels)
        {
            ensureCategoryAdded(category);

            categoryContentMap[category].AddRange(panels);
        }

        public bool SelectCategory(SidebarCategory category)
        {
            if (!categories.Contains(category))
                return false;

            activeCategory.Value = category;
            return true;
        }

        private void ensureCategoryAdded(SidebarCategory category)
        {
            if (!categories.Add(category))
                return;

            tabContent.Add(categoryContentMap[category] = new SidebarTabContent
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
            });

            buttonFlow.Insert(category.Priority, new SidebarCategoryButton(category, activeCategory));

            activeCategory.Value ??= category;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activeCategory.BindValueChanged(e =>
            {
                if (e.OldValue != null)
                {
                    bool willContract = e.NewValue == null;

                    categoryContentMap[e.OldValue].Hide(willContract);
                }

                if (e.NewValue != null)
                {
                    categoryContentMap[e.NewValue].Show();
                }

                expanded.Value = e.NewValue != null;
            }, true);

            expanded.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    tabContent.BypassAutoSizeAxes = Axes.None;
                    tabContent.FadeIn(200, Easing.Out);
                    samplePopIn?.Play();
                }
                else
                {
                    tabContent.BypassAutoSizeAxes = Axes.X;
                    tabContent.FadeOut(200, Easing.Out);
                    samplePopOut?.Play();
                }
            });
        }

        private partial class SidebarTabContent : Container
        {
            protected override Container<Drawable> Content { get; }

            public SidebarTabContent()
            {
                InternalChild = new OsuScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ScrollbarVisible = false,
                    Child = Content = new FillFlowContainer<Drawable>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5)
                    }
                };
            }

            public override void Show()
            {
                FinishTransforms();

                base.Show();
            }

            public new void Hide(bool animated = false)
            {
                FinishTransforms();

                if (!animated)
                    base.Hide();
                else
                    this.FadeOut(200);
            }
        }
    }
}
