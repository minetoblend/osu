// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Framework.Timing;
using osu.Game.Rulesets.Osu.Skinning;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.UI.Cursor
{
    public partial class CursorMotionBlur : CursorTrail
    {
        public double ShutterDuration
        {
            get
            {
                double fps = drawClock?.FramesPerSecond ?? 0;
                float refreshRate = displayMode?.Value.RefreshRate ?? 0;

                if (refreshRate > 0)
                {
                    if (fps > 0)
                        fps = Math.Min(fps, refreshRate);
                    else
                        fps = refreshRate;
                }

                return fps > 0 ? 1000 / fps : 1000 / 60.0;
            }
        }

        private IFrameBasedClock? drawClock;
        private IBindable<DisplayMode>? displayMode;

        public float Opacity => 0.2f;

        public float Spacing { get; set; } = 0.04f;

        protected override double FadeDuration => ShutterDuration;
        protected override float FadeExponent => 0.05f;

        protected override bool InterpolateMovements => true;
        protected override float IntervalMultiplier => Spacing * 2.5f;

        [Resolved]
        private ISkinSource skin { get; set; } = null!;

        public CursorMotionBlur()
        {
            Blending = BlendingParameters.Additive;
            Colour = Color4.White.Opacity(Opacity);
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            drawClock = host.DrawThread.Clock;
            displayMode = host.Window?.CurrentDisplayMode.GetBoundCopy();

            skin.SourceChanged += updateTexture;
            updateTexture();
        }

        private void updateTexture()
        {
            var texture = skin.GetTexture("cursor");

            if (texture == null)
            {
                Hide();
                return;
            }

            // stable "magic ratio", matching NonPlayfieldSprite.
            texture.ScaleAdjust *= 1.6f;

            Texture = texture;
            Show();

            bool centre = skin.GetConfig<OsuSkinConfiguration, bool>(OsuSkinConfiguration.CursorCentre)?.Value ?? true;
            TrailOrigin = centre ? Anchor.Centre : Anchor.TopLeft;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (skin.IsNotNull())
                skin.SourceChanged -= updateTexture;
        }
    }
}
