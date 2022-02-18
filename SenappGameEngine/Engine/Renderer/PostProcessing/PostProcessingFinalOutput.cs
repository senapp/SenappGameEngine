using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Renderer.Abstractions;
using Senapp.Engine.Shaders.PostProcessing;

namespace Senapp.Engine.Renderer.PostProcessing
{
    public class PostProcessingFinalOutput : IPostProcess
    {
        public PostProcessingFinalOutput()
        {
            shader = new PostProcessingOuputShader();
            renderer = new ImageRenderer();
        }

        public void OnResize(int width, int height) { }

        public int Render(int texture)
        {
            shader.Start();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            renderer.Render();
            shader.Stop();

            return texture;
        }

        public void Dispose()
        {
            renderer.Dispose();
            shader.Dispose();
        }

        private readonly ImageRenderer renderer;
        private readonly PostProcessingOuputShader shader;
    }
}
