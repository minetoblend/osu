// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Screens.Edit.Operators
{
    public abstract partial class OperatorOverlay : Container
    {
        public abstract LocalisableString Title { get; }

        private readonly FillFlowContainer content = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),
        };

        protected override Container<Drawable> Content => content;

        protected OperatorOverlay()
        {
            Masking = true;
            CornerRadius = 5;
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colourProvider.Background4,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 10),
                    Padding = new MarginPadding(5),
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Font = OsuFont.Style.Caption1.With(weight: FontWeight.Bold),
                        },
                        content,
                    },
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Apply();
        }

        protected void ParameterChanged<T>(ValueChangedEvent<T> _) => Scheduler.AddOnce(Apply);

        protected abstract void Apply();

        public virtual Drawable? CreatePlayfieldOverlay() => null;

        public virtual void OnComplete() { }
    }
}
