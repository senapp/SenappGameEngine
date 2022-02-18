using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Abstractions;

namespace Senapp.Engine.Renderer.PostProcessing
{
    public class PostProcessingManager
    {
        public PostProcessingManager()
        {
            Quad = Loader.LoadPositionsToVAO(POSITIONS, 2, "PostProcessQuad");

            //postProcesses.Add(new SSAO(Game.Instance.Width, Game.Instance.Height));
            postProcesses.Add(new PostProcessingFinalOutput()); // Needs to be last
        }

        public void OnResize(int width, int height)
        {
            foreach (var process in postProcesses)
            {
                process.OnResize(width, height);
            }
        }

        public void ApplyPostProcessing(int colourTexture)
        {
            Start();
            var passingTexture = colourTexture;
            foreach (var process in postProcesses)
            {
                passingTexture = process.Render(passingTexture);
            }
            Stop();
        }

        public void Dispose() 
        {
            foreach (var process in postProcesses)
            {
                process.Dispose();
            }
        }

        private void Start()
        {
            GL.BindVertexArray(Quad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.Disable(EnableCap.DepthTest);
        }
        private void Stop()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        private readonly RawModel Quad;
        private static readonly float[] POSITIONS = { -1, 1, -1, -1, 1, 1, 1, -1 };
        private readonly List<IPostProcess> postProcesses = new();
    }
}
