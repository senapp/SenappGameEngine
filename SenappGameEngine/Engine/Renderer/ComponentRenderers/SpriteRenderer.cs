using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Abstractions;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class SpriteRenderer : IComponentRenderer<Sprite, Texture>
    {
        public SpriteRenderer(SpriteShader shader)
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            this.shader = shader;
            this.shader.Start();
            this.shader.Stop();
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

        private readonly SpriteShader shader;
        private readonly RawModel UIQuad;
    }
}
