// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Screens.Edit.HitSounds.Components
{
    public partial class LedToggle : Checkbox
    {
        [Resolved]
        private OsuColour osuColour { get; set; } = null!;

        private readonly Circle circle;

        private Color4 enabledColour;
        private Color4 disabledColour;

        private Sample? sampleChecked;
        private Sample? sampleUnchecked;

        public LedToggle()
        {
            Size = new Vector2(20, 20);

            Padding = new MarginPadding(4);
            InternalChild = circle = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                BorderThickness = 4,
            };
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            enabledColour = osuColour.GreenLight;
            disabledColour = osuColour.Blue4;

            sampleChecked = audio.Samples.Get(@"UI/check-on");
            sampleUnchecked = audio.Samples.Get(@"UI/check-off");
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Current.BindValueChanged(updateState, true);
        }

        private void updateState(ValueChangedEvent<bool> state)
        {
            if (state.NewValue)
            {
                circle.Colour = enabledColour;
                circle.Blending = BlendingParameters.Additive;
                circle.EdgeEffect = new EdgeEffectParameters
                {
                    Radius = 32,
                    Colour = enabledColour.Opacity(0.1f),
                    Type = EdgeEffectType.Glow
                };
                circle.BorderColour = enabledColour.Opacity(0.8f);
            }
            else
            {
                circle.Colour = disabledColour;
                circle.Blending = BlendingParameters.Inherit;
                circle.EdgeEffect = default;
                circle.BorderColour = disabledColour.Opacity(0.5f);
            }
        }

        protected override void OnUserChange(bool value)
        {
            base.OnUserChange(value);

            if (value)
                sampleChecked?.Play();
            else
                sampleUnchecked?.Play();
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button == MouseButton.Left)
                circle.ScaleTo(0.9f, 300, Easing.OutExpo);

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            circle.ScaleTo(1f, 300, Easing.OutElasticHalf);

            base.OnMouseUp(e);
        }
    }
}
