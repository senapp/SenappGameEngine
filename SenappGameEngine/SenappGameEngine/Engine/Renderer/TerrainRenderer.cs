using Senapp.Engine.Shaders;
using Senapp.Engine.Terrains;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using Senapp.Engine.Models;
using Senapp.Engine.Base;

namespace Senapp.Engine.Renderer
{
    public class TerrainRenderer
    {
        private TerrainShader shader;
        public TerrainRenderer(TerrainShader _shader)
        {
            shader = _shader;
            shader.Start();
            shader.LoadTextureUnits();
            shader.Stop();
        }
        public void Render(List<GameObject> terrains)
        {
            foreach (GameObject terrain in terrains)
            {
                var terrainComponent = terrain.GetComponent<Terrain>();
                PrepareTerrain(terrainComponent);
                LoadModelMatrix(terrain);
                GL.DrawElements(BeginMode.Triangles, terrainComponent.model.vertexCount, DrawElementsType.UnsignedInt, 0);
                UnbindTexturedModel();
            }
        }
        private void PrepareTerrain(Terrain terrain)
        {
            RawModel model = terrain.model;
            GL.BindVertexArray(model.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            BindTextures(terrain);
            shader.LoadShineVariables(1, 0);
        }
        public void BindTextures(Terrain terrain)
        {
            TerrainTexture texturePack = terrain.texturePack;
            texturePack.backgroundTexture.Bind(TextureUnit.Texture0);
            texturePack.rTexture.Bind(TextureUnit.Texture1);
            texturePack.gTexture.Bind(TextureUnit.Texture2);
            texturePack.bTexture.Bind(TextureUnit.Texture3);
            texturePack.blendMap.Bind(TextureUnit.Texture4);

        }
        public void UnbindTexturedModel()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.BindVertexArray(0);
        }
        public void LoadModelMatrix(GameObject terrain)
        {
            shader.LoadTransformationMatrix(terrain.transform.TransformationMatrix());
        }
    }
}
