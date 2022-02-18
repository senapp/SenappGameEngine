using System;
using System.Drawing;
using System.Drawing.Imaging;

using Senapp.Engine.Loaders;
using OpenTK.Graphics.OpenGL;

namespace Senapp.Engine.Models
{
    public class Texture
    {
        public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)All.Srgb8Alpha8;
        public const SizedInternalFormat RGB32F = (SizedInternalFormat)All.Rgb32f;

        public const GetPName MAX_TEXTURE_MAX_ANISOTROPY = (GetPName)0x84FF;
        public const TextureParameterName TEXTURE_MAX_ANISOTROPY = (TextureParameterName)0x84FE;

        public static readonly float MaxAniso;

        static Texture()
        {
            MaxAniso = GL.GetFloat(MAX_TEXTURE_MAX_ANISOTROPY);
        }

        public readonly string Name;
        public readonly int GLTexture;
        public readonly int Width;
        public readonly int Height;
        public readonly int MipmapLevels;
        public readonly SizedInternalFormat InternalFormat;

        public Texture(string name, Bitmap image, bool generateMipmaps, bool srgb)
        {
            Name = name;
            Width = image.Width;
            Height = image.Height;
            InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;

            if (generateMipmaps)
            {
                MipmapLevels = (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));
            }
            else
            {
                MipmapLevels = 1;
            }

            GLTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, GLTexture);
            GL.TextureStorage2D(GLTexture, MipmapLevels, InternalFormat, Width, Height);

            BitmapData data = image.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GL.TextureSubImage2D(GLTexture, 0, 0, 0, Width, Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            if (generateMipmaps) GL.GenerateTextureMipmap(GLTexture);

            GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter, (int)(generateMipmaps ? TextureMinFilter.Linear : TextureMinFilter.LinearMipmapLinear));
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);

            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)OpenTK.Graphics.ES11.ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, MaxAniso);

            image.Dispose();
        }

        public void SetMinFilter(TextureMinFilter filter)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter, (int)filter);
        }
        public void SetMagFilter(TextureMagFilter filter)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)filter);
        }
        public void SetAnisotropy(float level)
        {
            GL.TextureParameter(GLTexture, TEXTURE_MAX_ANISOTROPY, Math.Clamp(level, 1, MaxAniso));
        }
        public void SetLod(int lod, int min, int max)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureLodBias, lod);
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinLod, min);
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLod, max);
        }      

        public void Bind(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, GLTexture);
        }
        public void Dispose()
        {
            Loader.DisposeTexture(this);
        }
    }
}
