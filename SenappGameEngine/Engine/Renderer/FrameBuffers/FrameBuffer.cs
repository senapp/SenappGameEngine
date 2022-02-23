using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;

namespace Senapp.Engine.Renderer.FrameBuffers
{
    public class ColourAttachment
    {
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat pixelFormat;
        public PixelType pixelType;

        public ColourAttachment(PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba8, PixelFormat pixelFormat = PixelFormat.Rgba, PixelType pixelType = PixelType.Float)
        {
            this.pixelInternalFormat = pixelInternalFormat;
            this.pixelFormat = pixelFormat;
            this.pixelType = pixelType;
        }
    }

    public class DepthAttachment
    {
        public bool renderBuffer;
        public PixelInternalFormat pixelInternalFormat;
        public PixelFormat pixelFormat;
        public FramebufferAttachment depthAttachment;
        public PixelType pixelType;

        public DepthAttachment(bool renderBuffer = false, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.DepthComponent24, PixelFormat pixelFormat = PixelFormat.DepthComponent, FramebufferAttachment depthAttachment = FramebufferAttachment.DepthAttachment, PixelType pixelType = PixelType.Float)
        {
            this.renderBuffer = renderBuffer;
            this.pixelInternalFormat = pixelInternalFormat;
            this.pixelFormat = pixelFormat;
            this.depthAttachment = depthAttachment;
            this.pixelType = pixelType;
        }
    }

    public class FrameBuffer
    {
        public static readonly float[] RENDER_QUAD_POSITIONS = { -1, 1, -1, -1, 1, 1, 1, -1 };

        public List<KeyValuePair<bool, int>> ColourAttachmentIds = new();
        public KeyValuePair<bool, int> DepthAttachmentId;

        public int Width = 0;
        public int Height = 0;
        public int Samples = 1;

        public List<ColourAttachment> ColourAttachments = new();
        public DepthAttachment DepthAttachment;

        private int frameBufferId;

        public FrameBuffer() { }

        public void Create()
        {
            if (frameBufferId != 0)
            {
                Dispose();
            }

            CreateFrameBuffer();
            bool multisample = Samples > 1;

            if (ColourAttachments.Any())
            {
                for (int i = 0; i < ColourAttachments.Count; i++)
                {
                    ColourAttachmentIds.Add(default);
                    CreateColourTextureOrBuffer(i, multisample);
                    AttachColourTextureOrBuffer(i, Samples, ColourAttachments[i]);
                }
            }

            if (DepthAttachment != null)
            {
                CreateDepthTextureOrBuffer(DepthAttachment.renderBuffer || multisample);
                AttachDepthTextureOrBuffer(Samples, DepthAttachment);
            }

            DrawBuffers();

            Unbind();
        }

        private void CreateColourTextureOrBuffer(int index, bool multisample)
        {
            if (!multisample)
            {
                ColourAttachmentIds[index] = new KeyValuePair<bool, int>(false, GL.GenTexture());
                GL.BindTexture(TextureTarget.Texture2D, ColourAttachmentIds[index].Value);
            }
            else
            {
                ColourAttachmentIds[index] = new KeyValuePair<bool, int>(true, GL.GenRenderbuffer());
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, ColourAttachmentIds[index].Value);
            }
        }
        private void CreateDepthTextureOrBuffer(bool renderBuffer)
        {
            if (renderBuffer)
            {
                DepthAttachmentId = new KeyValuePair<bool, int>(true, GL.GenRenderbuffer());
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthAttachmentId.Value);
            }
            else
            {
                DepthAttachmentId = new KeyValuePair<bool, int>(false, GL.GenTexture());
                GL.BindTexture(TextureTarget.Texture2D, DepthAttachmentId.Value);  
            }
        }

        private void AttachColourTextureOrBuffer(int index, int samples, ColourAttachment colourAttachment)
        {
            bool multisample = samples > 1;
            if (!multisample)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, colourAttachment.pixelInternalFormat, Width, Height, 0, colourAttachment.pixelFormat, colourAttachment.pixelType, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + index, TextureTarget.Texture2D, ColourAttachmentIds[index].Value, 0);
            }
            else
            {
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, (RenderbufferStorage)colourAttachment.pixelInternalFormat, Width, Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + index, RenderbufferTarget.Renderbuffer, ColourAttachmentIds[index].Value);
            }
        }
        private void AttachDepthTextureOrBuffer(int samples, DepthAttachment depthAttachment)
        {
            bool multisample = samples > 1;
            if (depthAttachment.renderBuffer)
            {
                if (!multisample)
                {
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage)depthAttachment.pixelInternalFormat, Width, Height);
                }
                else
                {
                    GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, (RenderbufferStorage)depthAttachment.pixelInternalFormat, Width, Height);
                }

                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, depthAttachment.depthAttachment, RenderbufferTarget.Renderbuffer, DepthAttachmentId.Value);
            }
            else
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, depthAttachment.pixelInternalFormat, Width, Height, 0, depthAttachment.pixelFormat, depthAttachment.pixelType, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, depthAttachment.depthAttachment, TextureTarget.Texture2D, DepthAttachmentId.Value, 0);
            }
        }

        private void CreateFrameBuffer()
        {
            frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
        }

        public void DrawBuffers()
        {
            if (ColourAttachments.Count > 1)
            {
                if (ColourAttachments.Count > 5) throw new Exception("[ENGINE][ERROR] To many colour attachments in FrameBuffer");
                var buffers = new List<DrawBuffersEnum>();
                for (int i = 0; i < ColourAttachments.Count; i++)
                {
                    buffers.Add(DrawBuffersEnum.ColorAttachment0 + i);
                }

                GL.DrawBuffers(ColourAttachments.Count, buffers.ToArray());
            }
            else if (ColourAttachments.Count == 0)
            {
                GL.DrawBuffer(DrawBufferMode.None);
            }
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, frameBufferId);
            DrawBuffers();
            GL.Viewport(0, 0, Width, Height);
        }
        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, Game.Instance.Width, Game.Instance.Height);
        }

        public void ResolveToFbo(FrameBuffer outputFbo)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, outputFbo.frameBufferId);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBufferId);

            for (int i = 0; i < Math.Min(ColourAttachments.Count, outputFbo.ColourAttachments.Count); i++)
            {
                GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + i);
                GL.DrawBuffer(DrawBufferMode.ColorAttachment0 + i);
                GL.BlitFramebuffer(0, 0, Width, Height, 0, 0, outputFbo.Width, outputFbo.Height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            }

            Unbind();
        }
        public int ReadPixel(int attachmentIndex, Vector2 position)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBufferId);
            int x = (int)position.X;
            int y = (int)position.Y;
            int pixelData = 0;
            if (attachmentIndex < ColourAttachmentIds.Count)
            {
                GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + attachmentIndex);
                GL.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.Int, ref pixelData);
            }
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            return pixelData;
        }
        public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            Create();
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(frameBufferId);
            foreach (var colourPair in ColourAttachmentIds)
            {
                if (colourPair.Key)
                {
                    GL.DeleteRenderbuffer(colourPair.Value);
                }
                else
                {
                    GL.DeleteTexture(colourPair.Value);
                }
            }
            if (DepthAttachmentId.Key)
            {
                GL.DeleteRenderbuffer(DepthAttachmentId.Value);
            }
            else
            {
                GL.DeleteTexture(DepthAttachmentId.Value);
            }

            ColourAttachmentIds.Clear();
            DepthAttachmentId = default;
        }
    }
}
