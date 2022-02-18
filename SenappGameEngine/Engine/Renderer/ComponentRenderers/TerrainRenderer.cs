using System.Collections.Generic;

using Senapp.Engine.Terrains;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Abstractions;
using Senapp.Engine.Shaders.Components;
using OpenTK.Graphics.OpenGL;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class TerrainRenderer : IComponentRenderer<Terrain, Terrain>
    {
        public TerrainRenderer(TerrainShader _shader)
        {
            shader = _shader;
            shader.Start();
            shader.LoadTextureUnits();
            shader.Stop();
        }

        public void Render(Dictionary<Terrain, List<Terrain>> terrains)
        {
            foreach (var id in terrains.Keys)
            {
                terrains.TryGetValue(id, out List<Terrain> batch);
                foreach (var terrain in batch)
                {
                    BindCommon(terrain);
                    PrepareInstance(terrain);
                    GL.DrawElements(BeginMode.Triangles, terrain.Model.VertexCount, DrawElementsType.UnsignedInt, 0);
                    UnbindCommon();
                }
            }
        }
        public void BindCommon(Terrain terrain)
        {
            RawModel model = terrain.Model;
            GL.BindVertexArray(model.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            BindTextures(terrain);
            shader.LoadShineVariables(1, 0);
        }
        public void BindTextures(Terrain terrain)
        {
            TerrainTexture texturePack = terrain.TexturePack;
            texturePack.backgroundTexture.Bind(TextureUnit.Texture0);
            texturePack.rTexture.Bind(TextureUnit.Texture1);
            texturePack.gTexture.Bind(TextureUnit.Texture2);
            texturePack.bTexture.Bind(TextureUnit.Texture3);
            texturePack.blendMap.Bind(TextureUnit.Texture4);
        }
        public void UnbindCommon()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.BindVertexArray(0);
        }
        public void PrepareInstance(Terrain terrain)
        {
            shader.LoadTransformationMatrix(terrain.gameObject.transform.CalculateTransformationMatrix());
        }

        private readonly TerrainShader shader;
    }
}
