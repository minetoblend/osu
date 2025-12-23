// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osuTK.Input;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    [Cached]
    public class PlayerCardHand : CardHand
    {
        public event Action? SelectionChanged;

        private CardSelectionMode selectionMode;

        public CardSelectionMode SelectionMode
        {
            get => selectionMode;
            set
            {
                selectionMode = value;
                allowSelection.Value = value != CardSelectionMode.Disabled;

                if (value == CardSelectionMode.Disabled)
                {
                    foreach (var card in Cards)
                        card.Selected = false;
                }
            }
        }

        private readonly BindableBool allowSelection = new BindableBool(true);

        protected override HandCard CreateCardFacade(RankedPlayCard card) => new PlayerHandCard(card)
        {
            Action = cardClicked,
            AllowSelection = allowSelection.GetBoundCopy(),
        };

        private IEnumerable<PlayerHandCard> selection => Cards.OfType<PlayerHandCard>().Where(it => it.Selected);

        public IEnumerable<RankedPlayCardWithPlaylistItem> Selection => selection.Select(it => it.Card.Item);

        private void cardClicked(PlayerHandCard card)
        {
            if (selectionMode == CardSelectionMode.Disabled)
                return;

            try
            {
                if (card.Selected)
                {
                    card.Selected = false;
                    return;
                }

                if (selectionMode == CardSelectionMode.Single)
                {
                    foreach (var c in Cards)
                    {
                        ((PlayerHandCard)c).Selected = c == card;
                    }
                }
                else
                {
                    card.Selected = true;
                }
            }
            finally
            {
                SelectionChanged?.Invoke();
            }
        }

        public partial class PlayerHandCard : HandCard
        {
            public required Action<PlayerHandCard> Action;

            public required IBindable<bool> AllowSelection;

            [Resolved]
            private PlayerCardHand cardHand { get; set; } = null!;

            public PlayerHandCard(RankedPlayCard card)
                : base(card)
            {
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                AddInternal(new HoverClickSounds
                {
                    Enabled = { BindTarget = AllowSelection }
                });
            }

            protected override bool OnHover(HoverEvent e)
            {
                CardHovered = true;

                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                CardHovered = false;
            }

            protected override bool OnMouseDown(MouseDownEvent e)
            {
                if (e.Button == MouseButton.Left && AllowSelection.Value)
                {
                    Card.ScaleTo(0.95f, 300, Easing.OutExpo);
                    return true;
                }

                return false;
            }

            protected override void OnMouseUp(MouseUpEvent e)
            {
                if (e.Button == MouseButton.Left)
                    Card.ScaleTo(1f, 400, Easing.OutElasticHalf);
            }

            protected override bool OnClick(ClickEvent e)
            {
                if (!AllowSelection.Value)
                    return false;

                Action(this);

                return true;
            }
        }
    }
}
