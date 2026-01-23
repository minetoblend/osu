// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay.Cards
{
    public class TiltContainer : Container, IBufferedDrawable
    {
        public Quaternion PerspectiveRotation = Quaternion.Identity;

        private IShader textureShader = null!;

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            textureShader = shaders.Load("RankedPlayCard", "RankedPlayCard");
        }

        protected override void Update()
        {
            base.Update();

            Invalidate(Invalidation.DrawNode);
        }

        IShader? ITexturedShaderDrawable.TextureShader => textureShader;

        public Color4 BackgroundColour { get; set; } = new Color4(0, 0, 0, 0);

        public Vector2 FrameBufferScale { get; set; } = Vector2.One;

        public float CameraDistance { get; set; } = 1500f;

        DrawColourInfo? IBufferedDrawable.FrameBufferDrawColour => base.DrawColourInfo;

        public override DrawColourInfo DrawColourInfo
        {
            get
            {
                // Todo: This is incorrect.
                var blending = Blending;
                blending.ApplyDefaultToInherited();

                return new DrawColourInfo(Color4.White, blending);
            }
        }

        public Vector3 NormalVector => Vector3.TransformNormal(Vector3.UnitZ, Matrix4.CreateFromQuaternion(PerspectiveRotation));

        private readonly BufferedDrawNodeSharedData sharedData = new BufferedDrawNodeSharedData();

        protected override DrawNode CreateDrawNode() => new TiltContainerDrawNode(this, sharedData);

        private class TiltContainerDrawNode : BufferedDrawNode, ICompositeDrawNode
        {
            protected new TiltContainer Source => (TiltContainer)base.Source;

            protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

            public TiltContainerDrawNode(TiltContainer source, BufferedDrawNodeSharedData sharedData)
                : base(source, new CompositeDrawableDrawNode(source), sharedData) { }

            private Quad screenSpaceDrawQuad;
            private Quad textureQuad;

            private IVertexBatch<CardVertex>? quadBatch;
            private Vector3 normalVector;
            private Matrix4 combinedMatrix;
            private Matrix4 modelViewMatrix;
            private float cameraDistance;

            public override void ApplyState()
            {
                base.ApplyState();

                screenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
                normalVector = Source.NormalVector;
                cameraDistance = Source.CameraDistance;

                // the card's content isn't always rendered upright, but we need the corners of the geometry to line up with the corners of the card
                textureQuad = screenSpaceDrawQuad.RelativeIn(DrawRectangle);

                // note: this works under the assumptions that we do not have any shear in our transformation
                var screenSpaceDirection = screenSpaceDrawQuad.TopRight - screenSpaceDrawQuad.TopLeft;
                float angle = MathF.Atan2(screenSpaceDirection.Y, screenSpaceDirection.X);

                Matrix4 model = Matrix4.CreateTranslation(new Vector3(-screenSpaceDrawQuad.Centre))
                                * Matrix4.CreateRotationZ(-angle)
                                * Matrix4.CreateFromQuaternion(Source.PerspectiveRotation)
                                * Matrix4.CreateRotationZ(angle);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 2, 1, 1, 5000);
                Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, cameraDistance), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

                modelViewMatrix = model * view;
                combinedMatrix = modelViewMatrix * projection;
            }

            private IUniformBuffer<CardEffectParameters>? parametersBuffer;

            protected override void DrawContents(IRenderer renderer)
            {
                quadBatch ??= renderer.CreateQuadBatch<CardVertex>(2, 1);

                renderer.BindTexture(SharedData.CurrentEffectBuffer.Texture);

                ColourInfo finalEffectColour = DrawColourInfo.Colour;

                quadBatch.Add(new CardVertex
                {
                    Position = screenSpaceDrawQuad.BottomLeft,
                    Colour = finalEffectColour.BottomLeft,
                    TexturePosition = applyPerspectiveCorrection(screenSpaceDrawQuad.BottomLeft, textureQuad.BottomLeft),
                });
                quadBatch.Add(new CardVertex
                {
                    Position = screenSpaceDrawQuad.BottomRight,
                    Colour = finalEffectColour.BottomRight,
                    TexturePosition = applyPerspectiveCorrection(screenSpaceDrawQuad.BottomRight, textureQuad.BottomRight),
                });
                quadBatch.Add(new CardVertex
                {
                    Position = screenSpaceDrawQuad.TopRight,
                    Colour = finalEffectColour.TopRight,
                    TexturePosition = applyPerspectiveCorrection(screenSpaceDrawQuad.TopRight, textureQuad.TopRight),
                });
                quadBatch.Add(new CardVertex
                {
                    Position = screenSpaceDrawQuad.TopLeft,
                    Colour = finalEffectColour.TopLeft,
                    TexturePosition = applyPerspectiveCorrection(screenSpaceDrawQuad.TopLeft, textureQuad.TopLeft),
                });

                Vector3 applyPerspectiveCorrection(Vector2 screenPosition, Vector2 textureCoord)
                {
                    var v4 = Vector4.Transform(new Vector4(screenPosition.X, screenPosition.Y, 0, 1), combinedMatrix);
                    return new Vector3(textureCoord.X / v4.W, textureCoord.Y / v4.W, 1f / v4.W);
                }
            }

            protected override void BindUniformResources(IShader shader, IRenderer renderer)
            {
                base.BindUniformResources(shader, renderer);

                parametersBuffer ??= renderer.CreateUniformBuffer<CardEffectParameters>();
                parametersBuffer.Data = new CardEffectParameters
                {
                    NormalVector = new Vector4(normalVector),
                    CombinedMatrix = combinedMatrix,
                    ModelViewMatrix = modelViewMatrix,
                    Centre = screenSpaceDrawQuad.Centre,
                    Distance = cameraDistance,
                };

                shader.BindUniformBlock("m_CardEffectParameters", parametersBuffer);
            }

            public List<DrawNode>? Children
            {
                get => Child.Children;
                set => Child.Children = value;
            }

            public bool AddChildDrawNodes => true;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private record struct CardEffectParameters
            {
                public UniformVector4 NormalVector;
                public UniformMatrix4 CombinedMatrix;
                public UniformMatrix4 ModelViewMatrix;
                public UniformVector2 Centre;
                public UniformFloat Distance;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct CardVertex : IEquatable<CardVertex>, IVertex
            {
                [VertexMember(2, VertexAttribPointerType.Float)]
                public Vector2 Position;

                [VertexMember(4, VertexAttribPointerType.Float)]
                public Color4 Colour;

                [VertexMember(3, VertexAttribPointerType.Float)]
                public Vector3 TexturePosition;

                public bool Equals(CardVertex other) =>
                    Position.Equals(other.Position)
                    && Colour.Equals(other.Colour)
                    && TexturePosition.Equals(other.TexturePosition);
            }

            protected override void Dispose(bool isDisposing)
            {
                quadBatch?.Dispose();
                parametersBuffer?.Dispose();

                base.Dispose(isDisposing);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            sharedData.Dispose();

            base.Dispose(isDisposing);
        }
    }

    static file class Extensions
    {
        public static Vector2 RelativeIn(this Vector2 v, RectangleF rect) => new Vector2((v.X - rect.Left) / rect.Width, (v.Y - rect.Top) / rect.Height);

        public static Quad RelativeIn(this Quad quad, RectangleF rect) => new Quad(
            quad.TopLeft.RelativeIn(rect),
            quad.TopRight.RelativeIn(rect),
            quad.BottomLeft.RelativeIn(rect),
            quad.BottomRight.RelativeIn(rect)
        );
    }
}
