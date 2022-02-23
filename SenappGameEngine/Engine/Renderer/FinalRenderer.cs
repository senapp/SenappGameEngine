﻿using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Renderer.FrameBuffers;
using Senapp.Engine.Shaders;

namespace Senapp.Engine.Renderer
{
    public class FinalRenderer : FrameBufferRenderer
    {
        public FinalRenderer()
        {
            shader = new FinalShader();
        }

        public int ColourAttachmentId = 0;

        public void Render(FrameBuffer frameBuffer)
        {
            shader.Start();
            if (ColourAttachmentId >= frameBuffer.ColourAttachmentIds.Count || ColourAttachmentId < 0) ColourAttachmentId = 0;
            Bind(frameBuffer.ColourAttachmentIds[ColourAttachmentId].Value);
            base.Render();
            Unbind();

            shader.Stop();
        }

        public void Dispose()
        {
            shader.Dispose();
        }

        public void Bind(int colourTexture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colourTexture);
        }
        private void Unbind()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private readonly FinalShader shader;
    }
}
