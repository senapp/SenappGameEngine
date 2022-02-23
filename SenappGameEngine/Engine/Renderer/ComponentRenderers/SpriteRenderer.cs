using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Entities;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class SpriteRenderer
    {
        public SpriteRenderer()
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            shader = new SpriteShader();
        }

        public void Start(Camera camera)
        {
            shader.Start();
            shader.LoadCameraMatrix(camera);
        }
        public void Stop()
        {
            shader.Stop();
        }
        public void Render(Dictionary<Texture, List<Sprite>> sprites)
        {
            foreach (Texture texture in sprites.Keys)
            {
                BindCommon(texture);
                sprites.TryGetValue(texture, out List<Sprite> batch);
                foreach (var sprite in batch)
                {
                    PrepareInstance(sprite);
                    GL.DrawElements(BeginMode.Triangles, UIQuad.VertexCount, DrawElementsType.UnsignedInt, 0);
                }
                UnbindCommon();
            }
        }
        public void BindCommon(Texture texture)
        {
            texture.Bind(TextureUnit.Texture0);
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(UIQuad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }
        public void UnbindCommon()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest); 
        }
        public void PrepareInstance(Sprite sprite)
        {
            shader.LoadTransformationMatrix(
                sprite.gameObject.transform.CalculateTransformationMatrix(
                    Game.Instance.MainCamera.gameObject.transform.GetWorldPosition(), null, sprite));
            shader.LoadColour(sprite.gameObject.colour.ToVector());
        }
        public void Dispose()
        {
            shader.Dispose();
            UIQuad.Dispose();
        }

        private readonly SpriteShader shader;
        private readonly RawModel UIQuad;
    }
}
