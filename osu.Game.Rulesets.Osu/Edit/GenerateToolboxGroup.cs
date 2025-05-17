// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Edit;
using osu.Game.Screens.Edit.Components;
using osuTK;
using osuTK.Input;

namespace osu.Game.Rulesets.Osu.Edit
{
    public partial class GenerateToolboxGroup : EditorToolboxGroup
    {
        private readonly EditorToolButton polygonButton;

        private readonly EditorToolButton copilotButton;

        [Resolved]
        private OsuCopilot copilot { get; set; }

        public GenerateToolboxGroup()
            : base("Generate")
        {
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(5),
                Children = new Drawable[]
                {
                    polygonButton = new EditorToolButton("Polygon",
                        () => new SpriteIcon { Icon = FontAwesome.Solid.Spinner },
                        () => new PolygonGenerationPopover()),
                    copilotButton = new EditorToolButton(
                        "Copilot",
                        () => new SpriteIcon { Icon = FontAwesome.Solid.Magic },
                        () => new CopilotPopover(copilot)
                    )
                }
            };
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Repeat) return false;

            switch (e.Key)
            {
                case Key.D:
                    if (!e.ControlPressed || !e.ShiftPressed)
                        return false;

                    polygonButton.TriggerClick();
                    return true;

                default:
                    return false;
            }
        }

        private partial class CopilotPopover : OsuPopover
        {
            public CopilotPopover(OsuCopilot copilot)
            {
                this.copilot = copilot;
            }

            private LoadingSpinner spinner;

            private Bindable<bool> requestActive;

            private OsuCopilot copilot { get; set; }

            [BackgroundDependencyLoader]
            private void load()
            {
                Add(new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(10),
                    Children =
                    [
                        new OsuSpriteText
                        {
                            Text = "Generating...",
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                        },
                        spinner = new LoadingSpinner
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(12),
                            Depth = -1,
                        }
                    ]
                });

                requestActive = copilot.RequestActive.GetBoundCopy();
                requestActive.BindValueChanged(active =>
                {
                    if (active.NewValue)
                        spinner.Show();
                    else
                    {
                        spinner.Hide();
                        this.HidePopover();
                    }
                });

                copilot.SendRequest();
            }
        }
    }
}
