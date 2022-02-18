using System;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer.PostProcessing
{
    public enum DepthBufferType
    {
        NONE = 0,
        DEPTH_TEXTURE = 1,
        DEPTH_RENDER_BUFFER = 2
    }

    public class Fbo
    {
        public int width;
        public int height;

        public int ColourTexture { get; private set; }
        public int DepthTexture { get; private set; }

        public Fbo(int width, int height, DepthBufferType depthBufferType)
        {
            this.width = width;
            this.height = height;
            InitialiseFrameBuffer(depthBufferType);
        }

        public Fbo(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.multisampled = true;
            InitialiseFrameBuffer(DepthBufferType.DEPTH_RENDER_BUFFER);
        }

        public void ResolveToFbo(Fbo outputFbo)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, outputFbo.frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBuffer);
            GL.BlitFramebuffer(0, 0, width, height, 0, 0, outputFbo.width, outputFbo.height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            Unbind();
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, frameBuffer);
            GL.Viewport(0, 0, width, height);
        }
        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Game.Instance.Width, Game.Instance.Height);
        }
        public void BindToRead()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBuffer);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(frameBuffer);
            GL.DeleteTexture(ColourTexture);
            GL.DeleteTexture(DepthTexture);
            GL.DeleteRenderbuffer(colourBuffer);
            GL.DeleteRenderbuffer(depthBuffer);
        }

        private void InitialiseFrameBuffer(DepthBufferType type)
        {
            CreateFrameBuffer();
            if (!multisampled)
            {
                CreateTextureAttachment();
            }
            else
            {
                CreateMultisampleColourAttachment();
            }

            switch (type)
            {
                case DepthBufferType.NONE:
                    break;
                case DepthBufferType.DEPTH_TEXTURE:
                    CreateDepthTextureAttachment();
                    break;
                case DepthBufferType.DEPTH_RENDER_BUFFER:
                    CreateDepthBufferAttachment();
                    break;
            }

            Unbind();
        }

        private void CreateFrameBuffer()
        {
            frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }
        private void CreateTextureAttachment()
        {
            ColourTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColourTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColourTexture, 0);
        }
        private void CreateDepthTextureAttachment()
        {
            DepthTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTexture, 0);
        }
        private void CreateMultisampleColourAttachment()
        {
            colourBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colourBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, GraphicsSettings.AntiAliasing == AntiAliasingTypes.MSAA ? GraphicsSettings.MSAASamples : 0, RenderbufferStorage.Rgba8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, colourBuffer);
        }
        private void CreateDepthBufferAttachment()
        {
            depthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            if (!multisampled)
            {
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            }
            else
            {
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, GraphicsSettings.AntiAliasing == AntiAliasingTypes.MSAA ? GraphicsSettings.MSAASamples : 0, RenderbufferStorage.DepthComponent24, width, height);
            }
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
        }

        private bool multisampled;
        private int frameBuffer;
        private int colourBuffer;
        private int depthBuffer;
    }
}
