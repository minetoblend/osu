// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardHand
    {
        public partial class PlayerCard : CompositeDrawable
        {
            public readonly RankedPlayCardItem Item;

            public bool Selected;

            public CardState State = CardState.Hand;

            public readonly BindableBool AllowSelection = new BindableBool();

            public PlayerCard(RankedPlayCardItem item)
            {
                Item = item;
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                Size = new Vector2(150, 250) + new Vector2(20);
                Padding = new MarginPadding(10);

                InternalChild = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 20,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Radius = 10,
                        Colour = Color4.Black.Opacity(0.1f),
                    },
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Gray,
                    }
                };
            }

            private Vector2 velocity;
            private float rotationVelocity;

            public void UpdateMovement(Vector2 targetPosition, float targetRotation)
            {
                var spring = State switch
                {
                    CardState.Hand => CardMovement.ENERGETIC,
                    CardState.Lineup => CardMovement.SMOOTH,
                    CardState.NewlyDrawn => CardMovement.SMOOTH,
                    CardState.Hidden => CardMovement.ENERGETIC,

                    _ => throw new ArgumentOutOfRangeException()
                };

                Position = spring.Update(Time.Elapsed, current: Position, target: targetPosition, velocity: ref velocity);
                Rotation = spring.Update(Time.Elapsed, current: Rotation, target: targetRotation, velocity: ref rotationVelocity);
            }

            public float CardLayoutWidth => DrawWidth * DrawScale.X;

            protected override bool OnHover(HoverEvent e)
            {
                this.ScaleTo(1.1f, 400, Easing.OutExpo);

                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);

                this.ScaleTo(1, 400, Easing.OutExpo);
            }

            protected override bool OnClick(ClickEvent e)
            {
                if (AllowSelection.Value)
                {
                    Selected = !Selected;
                    return true;
                }

                return base.OnClick(e);
            }

            private class StateTransform : Transform<CardState, PlayerCard>
            {
                public override string TargetMember => nameof(State);

                protected override void Apply(PlayerCard d, double time)
                {
                    if (time >= EndTime)
                        Target.State = EndValue;
                }

                protected override void ReadIntoStartValue(PlayerCard d)
                {
                    StartValue = d.State;
                }
            }

            public TransformSequence<PlayerCard> ChangeStateTo(CardState state) => this.TransformTo(this.PopulateTransform(new StateTransform(), state));
        }
    }

    internal static class CardHandExtensions
    {
        public static TransformSequence<PlayerCardHand.PlayerCard> ChangeStateTo(this TransformSequence<PlayerCardHand.PlayerCard> sequence, PlayerCardHand.CardState state) =>
            sequence.Append(o => o.ChangeStateTo(state));
    }
}
