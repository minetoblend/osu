// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Overlays;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Edit.Tools
{
    public partial class ComposeToolButton : CompositeDrawable, IHasTooltip
    {
        public readonly ComposeToolInfo ToolInfo;

        private Container content = null!;
        private Drawable icon = null!;
        private Box background = null!;

        [Resolved]
        private HitObjectComposer composer { get; set; } = null!;

        [Resolved]
        private IBindable<ComposeToolInfo> activeTool { get; set; } = null!;

        [Resolved]
        private OverlayColourProvider colourProvider { get; set; } = null!;

        private readonly BindableBool selected = new BindableBool();

        public ComposeToolButton(ComposeToolInfo toolInfo)
        {
            ToolInfo = toolInfo;
            Size = ComposeToolbar.ButtonSize;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren =
            [
                content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    CornerRadius = 6,
                    Children =
                    [
                        background = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = colourProvider.Background3,
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = icon = (ToolInfo.CreateIcon() ?? Empty()).With(d =>
                            {
                                d.Anchor = Anchor.Centre;
                                d.Origin = Anchor.Centre;
                                d.Size = new Vector2(24);
                            })
                        }
                    ],
                }
            ];
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activeTool.BindValueChanged(e => selected.Value = e.NewValue == ToolInfo, true);

            selected.BindValueChanged(e =>
            {
                background.Colour = e.NewValue ? colourProvider.Background3 : colourProvider.Background4;
                icon.Alpha = e.NewValue ? 1 : 0.5f;
            }, true);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button != MouseButton.Left)
                return false;

            content.ScaleTo(0.9f, 400, Easing.OutExpo);
            icon.ScaleTo(0.9f, 400, Easing.OutExpo);

            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (e.Button == MouseButton.Left)
            {
                content.ScaleTo(1, 400, Easing.OutElasticHalf);
                icon.ScaleTo(1, 400, Easing.OutElasticHalf);
            }
        }

        protected override bool OnClick(ClickEvent e)
        {
            composer.SetActiveTool(ToolInfo);
            return true;
        }

        public LocalisableString TooltipText => ToolInfo.Name;
    }
}
