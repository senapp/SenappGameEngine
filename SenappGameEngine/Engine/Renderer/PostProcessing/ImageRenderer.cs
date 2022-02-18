using OpenTK.Graphics.OpenGL;

namespace Senapp.Engine.Renderer.PostProcessing
{
    public class ImageRenderer
    {
        public int OutputTexture => imageFbo?.ColourTexture ?? 0;

        public ImageRenderer(int width, int height)
        {
            imageFbo = new Fbo(width, height, DepthBufferType.NONE);
        }
        public ImageRenderer() { }

        public void Render()
        {
            imageFbo?.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            imageFbo?.Unbind();
        }

        public void Dispose()
        {
            imageFbo?.Dispose();
        }

        private readonly Fbo imageFbo;
    }
}
