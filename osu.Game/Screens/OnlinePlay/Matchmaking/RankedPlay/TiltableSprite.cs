// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Game.Online.API.Requests.Responses;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public class TiltableSprite : BufferedContainer
    {
        private RankedPlayCard card;

        public TiltableSprite(APIBeatmap beatmap)
        {
            Add(card = new RankedPlayCard(beatmap));
            AutoSizeAxes = Axes.Both;
        }

        protected override void Update()
        {
            base.Update();

            actualMousePos.Update((float)(Time.Elapsed / 1000), mousePos);

            card.Parallax = actualMousePos.Current * new Vector2(1, -1) / 10;

            Invalidate(Invalidation.DrawNode | Invalidation.MiscGeometry);
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.FlashColour(Color4.Red, 100);

            return base.OnHover(e);
        }

        private Vector2 mousePos;

        private SecondOrderDynamics actualMousePos = new SecondOrderDynamics(new Vector2(), new DynamicsParameters(
            Frequency: 1,
            Damping: 1,
            Response: 1
        ));

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            mousePos = ToLocalSpace(e.ScreenSpaceMousePosition) - DrawSize / 2;

            Logger.Log($"{mousePos}");

            return base.OnMouseMove(e);
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        {
            return true;
        }

        protected Quad ComputePerspectiveDrawQuad()
        {
            var quad = base.ComputeScreenSpaceDrawQuad();

            var center = new Vector3(quad.Centre);

            float angle = MathHelper.DegreesToRadians((float)(Time.Current * 0.08));

            Matrix4 model =
                Matrix4.CreateTranslation(-center) *
                Matrix4.LookAt(new Vector3(), new Vector3(actualMousePos.Current.X, -actualMousePos.Current.Y, 400), new Vector3(0, 1, 0)) *
                Matrix4.CreateTranslation(center);

            return new Quad(
                transform(quad.TopLeft),
                transform(quad.TopRight),
                transform(quad.BottomLeft),
                transform(quad.BottomRight)
            );

            Vector2 transform(Vector2 v)
            {
                var pos = Vector3.TransformPosition(new Vector3(v), model);

                return (center + (pos - center) / (pos.Z + 1000) * 1000).Xy;
            }
        }

        protected override DrawNode CreateDrawNode() => new TiltableSpriteDrawNode(this, sharedData);

        private class TiltableSpriteDrawNode : BufferedContainerDrawNode
        {
            protected new TiltableSprite Source => (TiltableSprite)base.Source;

            public TiltableSpriteDrawNode(BufferedContainer<Drawable> source, BufferedContainerDrawNodeSharedData sharedData)
                : base(source, sharedData) { }

            private Quad perspectiveQuad;

            public override void ApplyState()
            {
                base.ApplyState();

                perspectiveQuad = Source.ComputePerspectiveDrawQuad();
            }

            protected override void DrawContents(IRenderer renderer)
            {
                renderer.SetBlend(effectBlending);

                ColourInfo finalEffectColour = DrawColourInfo.Colour;
                finalEffectColour.ApplyChild(effectColour);

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        var rect = new RectangleF(x / 10f, y / 10f, 0.1f, 0.1f);

                        var quad = slice(perspectiveQuad, rect);

                        renderer.DrawQuad(SharedData.CurrentEffectBuffer.Texture, quad, finalEffectColour, rect * SharedData.CurrentEffectBuffer.Texture.DisplaySize);
                    }
                }

                // renderer.DrawFrameBuffer(SharedData.CurrentEffectBuffer, perspectiveQuad, finalEffectColour);

                static Quad slice(Quad quad, RectangleF range) =>
                    verticalSlice(
                        horizontalSlice(quad, range.Left, range.Right),
                        range.Top,
                        range.Bottom
                    );

                static Quad horizontalSlice(Quad quad, float start, float end) =>
                    new Quad(
                        Vector2.Lerp(quad.TopLeft, quad.TopRight, start),
                        Vector2.Lerp(quad.TopLeft, quad.TopRight, end),
                        Vector2.Lerp(quad.BottomLeft, quad.BottomRight, start),
                        Vector2.Lerp(quad.BottomLeft, quad.BottomRight, end)
                    );

                static Quad verticalSlice(Quad quad, float start, float end) =>
                    new Quad(
                        Vector2.Lerp(quad.TopLeft, quad.BottomLeft, start),
                        Vector2.Lerp(quad.TopRight, quad.BottomRight, start),
                        Vector2.Lerp(quad.TopLeft, quad.BottomLeft, end),
                        Vector2.Lerp(quad.TopRight, quad.BottomRight, end)
                    );
            }
        }
    }
}
