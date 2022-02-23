using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Loaders;
using Senapp.Engine.Models;

namespace Senapp.Engine.Renderer.FrameBuffers
{
    public abstract class FrameBufferRenderer
    {
        public FrameBufferRenderer()
        {
            Quad = Loader.LoadPositionsToVAO(FrameBuffer.RENDER_QUAD_POSITIONS, 2, "FrameBufferQuad");
        }

        internal void Render()
        {
            BindQuad();
            RenderQuad();
            UnbindQuad();
        }

        private void BindQuad()
        {
            GL.BindVertexArray(Quad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.Disable(EnableCap.DepthTest);
        }
        private void UnbindQuad()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
        private void RenderQuad()
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        private readonly RawModel Quad;
    }
}
