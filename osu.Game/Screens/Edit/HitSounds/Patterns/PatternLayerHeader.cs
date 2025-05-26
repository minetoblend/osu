// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps.HitSounds;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Screens.Edit.HitSounds.Components;
using osuTK;

namespace osu.Game.Screens.Edit.HitSounds.Patterns
{
    public partial class PatternLayerHeader : CompositeDrawable
    {
        private readonly HitSoundLayer layer;

        private readonly Bindable<bool> enabled = new Bindable<bool>();

        private OsuRearrangeableListItem<HitSoundLayer>.PlaylistItemHandle handle = null!;
        private LayerSampleSelect sampleSelect = null!;

        [Resolved]
        private Bindable<HitSoundLayer?> draggedLayer { get; set; } = null!;

        [Resolved]
        private PatternLayer drawableLayer { get; set; } = null!;

        public PatternLayerHeader(HitSoundLayer layer)
        {
            this.layer = layer;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colour)
        {
            RelativeSizeAxes = Axes.Both;
            Padding = new MarginPadding(4);

            enabled.BindTo(drawableLayer.Enabled);

            InternalChildren = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(4),
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Alpha = 0.5f,
                            Child = handle = new OsuRearrangeableListItem<HitSoundLayer>.PlaylistItemHandle
                            {
                                Size = new Vector2(12),
                                AlwaysPresent = true,
                                Alpha = 0,
                            },
                        },
                        new LedToggle
                        {
                            Current = enabled,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        sampleSelect = new LayerSampleSelect(layer)
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            draggedLayer.BindValueChanged(_ => updateHandle(), true);

            enabled.BindValueChanged(enabled => sampleSelect.FadeTo(enabled.NewValue ? 1 : 0.5f));
        }

        protected override bool OnClick(ClickEvent e)
        {
            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateHandle();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateHandle();
        }

        private void updateHandle()
        {
            bool showHandle = IsHovered;

            if (draggedLayer.Value != null)
                showHandle = draggedLayer.Value == layer;

            handle.UpdateHoverState(showHandle);
        }

        public bool HandlingDrag => handle.HandlingDrag;
    }
}
