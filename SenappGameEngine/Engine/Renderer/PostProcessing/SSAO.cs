using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Renderer.Abstractions;
using Senapp.Engine.Shaders.PostProcessing;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer.PostProcessing
{
    public class SSAO : IPostProcess
    {
        public const int SAMPLE_POINTS = 64;

        public SSAO(int width, int height)
        {
            shader = new SSAOShader();
            renderer = new ImageRenderer(width, height);

            shader.Start();
            for (int i = 0; i < SAMPLE_POINTS; i++)
            {
                var vec = new Vector3(Randomize.RangeFloat(0, 1) * 2 - 1, Randomize.RangeFloat(0, 1) * 2 - 1, Randomize.RangeFloat(0, 1));
                var sample = vec.Normalized();
                sample *= Randomize.RangeFloat(0, 1);

                float scale = i / 64f;
                sample *= Mathematics.Lerp(0.1f, 1f, scale * scale);
                shader.LoadSample(i, sample);
            }
            List<Vector3> ssaoNoise = new();
            for (int i = 0; i < 16; i++)
            {
                var noise = new Vector3(Randomize.RangeFloat(0, 1) * 2 - 1, Randomize.RangeFloat(0, 1) * 2 - 1, 0);
                ssaoNoise.Add(noise);
            }
            CreateNoiseTexture(ssaoNoise);
            shader.Stop();
        }

        private void CreateNoiseTexture(List<Vector3> ssaoNoise)
        {
            noiseTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, noiseTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, 4, 4, 0, PixelFormat.Rgb, PixelType.Float, (IntPtr)ssaoNoise[0].ToFloats()[0]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        public void OnResize(int width, int height)
        {
            renderer.Dispose();
            renderer = new ImageRenderer(width, height);
        }

        public int Render(int texture)
        {
            shader.Start();
            shader.LoadProjectionMatrix(Game.Instance.MainCamera.GetProjectionMatrix());
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            renderer.Render();
            shader.Stop();

            return renderer.OutputTexture;
        }

        public void Dispose()
        {
            GL.DeleteTexture(noiseTexture);
            renderer.Dispose();
            shader.Dispose();
        }

        private ImageRenderer renderer;
        private readonly SSAOShader shader;
        private int noiseTexture;
    }
}
