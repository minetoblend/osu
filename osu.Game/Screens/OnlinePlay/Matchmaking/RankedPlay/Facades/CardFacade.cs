// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public partial class CardFacade : CompositeDrawable
    {
        public bool Selected;

        [Resolved(name: "debugEnabled")]
        private Bindable<bool>? debugEnabled { get; set; }

        public readonly Bindable<SpringParameters> CardMovementBindable = new Bindable<SpringParameters>(RankedPlayScreen.MovementStyle.Smooth);

        public SpringParameters CardMovement
        {
            get => CardMovementBindable.Value;
            set => CardMovementBindable.Value = value;
        }

        protected readonly Container DebugOverlay;

        public CardFacade()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Size = new Vector2(120, 200);
            Padding = new MarginPadding(-10);

            InternalChild = DebugOverlay = new Container
            {
                RelativeSizeAxes = Axes.Both,

                Masking = true,
                MaskingSmoothness = 1,

                BorderColour = Color4.Red,
                BorderThickness = 1.5f,
                Padding = new MarginPadding(-1),
                Alpha = 0,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.05f,
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            debugEnabled?.BindValueChanged(e => DebugOverlay.Alpha = e.NewValue ? 1 : 0, true);
        }

        public virtual bool OnCardHover(HoverEvent e) => false;

        public virtual void OnCardHoverLost(HoverLostEvent e) { }

        public virtual bool OnCardClicked(ClickEvent e) => false;

        public virtual bool OnCardMouseDown(MouseDownEvent e) => false;

        public virtual void OnCardMouseUp(MouseUpEvent e) { }

        public virtual bool OnCardDragStart(DragStartEvent e) => false;

        public virtual void OnCardDrag(DragEvent e) { }

        public virtual void OnCardDragEnd(DragEndEvent e) { }
    }
}
