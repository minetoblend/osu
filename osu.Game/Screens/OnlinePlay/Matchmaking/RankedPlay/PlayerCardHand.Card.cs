// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Online.Multiplayer.MatchTypes.RankedPlay;
using osu.Game.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class PlayerCardHand
    {
        public partial class PlayerCard : CompositeDrawable
        {
            public readonly RankedPlayCard Item;

            public bool NewlyAdded;
            public bool Selected;

            public readonly BindableBool AllowSelection = new BindableBool();

            public PlayerCard(RankedPlayCard item)
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

            public void UpdateMovement(in Spring spring, Vector2 targetPosition, float targetRotation)
            {
                Position = spring.Update(Time.Elapsed, current: Position, target: targetPosition, velocity: ref velocity);
                Rotation = spring.Update(Time.Elapsed, current: Rotation, target: targetRotation, velocity: ref rotationVelocity);
            }

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
        }
    }
}
