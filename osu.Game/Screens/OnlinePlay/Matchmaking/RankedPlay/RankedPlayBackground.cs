// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Game.Graphics.Backgrounds;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.RankedPlay
{
    public partial class RankedPlayBackground : CompositeDrawable, IBufferedDrawable
    {
        public float Spacing = 10;

        public IShader? TextureShader { get; private set; }
        public Color4 BackgroundColour => Color4.Black;
        public DrawColourInfo? FrameBufferDrawColour => null;
        public Vector2 FrameBufferScale => new Vector2(0.25f);

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);

            InternalChild = new TrianglesV2
            {
                RelativeSizeAxes = Axes.Both,
                ScaleAdjust = 2,
                Thickness = 0.05f,
            };
        }

        private readonly BufferedDrawNodeSharedData sharedData = new BufferedDrawNodeSharedData();

        protected override DrawNode CreateDrawNode()
        {
            return new RankedPlayBackgroundDrawNode(this, sharedData);
        }

        private class RankedPlayBackgroundDrawNode : BufferedDrawNode, ICompositeDrawNode
        {
            private const int max_sprites = 20_000;

            protected new RankedPlayBackground Source => (RankedPlayBackground)base.Source;

            protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

            public RankedPlayBackgroundDrawNode(RankedPlayBackground source, BufferedDrawNodeSharedData sharedData)
                : base(source, new CompositeDrawableDrawNode(source), sharedData)
            {
            }

            private int rows;
            private int cols;
            private Vector2 spacing;
            private Part[] parts = [];

            public override void ApplyState()
            {
                base.ApplyState();

                const float hex_ratio = 0.86f;

                spacing = new Vector2(
                    Source.Spacing / Source.DrawWidth * DrawRectangle.Width,
                    Source.Spacing / Source.DrawHeight * DrawRectangle.Height * hex_ratio
                );
                rows = (int)(DrawRectangle.Height / spacing.Y);
                cols = (int)(DrawRectangle.Width / spacing.X);

                if (parts.Length != cols * rows)
                    parts = new Part[rows * cols];

                var cellSize = new Vector2(spacing.X);

                for (int i = 0; i < parts.Length; i++)
                {
                    int row = i / cols;
                    int col = i % cols;

                    var position = new Vector2(col, row) * spacing;

                    if ((row & 0x01) == 0)
                        position.X += spacing.X / 2;

                    var drawQuad = new RectangleF(DrawRectangle.TopLeft + position - cellSize * 0.5f, cellSize);
                    var uvs = new RectangleF(new Vector2((float)col / cols, (float)row / cols), Vector2.Zero);

                    parts[i] = new Part(drawQuad, uvs);
                }
            }

            protected override void DrawContents(IRenderer renderer)
            {
                var texture = renderer.WhitePixel;

                foreach (var part in parts)
                {
                    renderer.DrawQuad(texture, vertexQuad: part.DrawQuad, drawColour: Color4.White, textureCoords: part.UVs);
                }
            }

            public List<DrawNode>? Children
            {
                get => Child.Children;
                set => Child.Children = value;
            }

            public bool AddChildDrawNodes => RequiresRedraw;

            private readonly record struct Part(Quad DrawQuad, RectangleF UVs);
        }
    }
}
