// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Utils;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Facades
{
    public partial class CardFacade : CompositeDrawable
    {
        public bool Selected;

        public readonly Bindable<SpringParameters> CardMovementBindable = new Bindable<SpringParameters>(RankedPlayScreen.MovementStyle.Smooth);

        public float Elevation;

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

            Size = RankedPlayScreen.Card.SIZE;
            Padding = new MarginPadding(-10);

            AddInternal(DebugOverlay = new DebugBox());
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
