using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Models;
using Senapp.Engine.Utilities;
using Senapp.Engine.Entities;
using Senapp.Engine.Renderer.Abstractions;
using Senapp.Engine.Shaders.Components;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class EntityRenderer : IComponentRenderer<Entity, TexturedModel>
    {
        public EntityRenderer(EntityShader _shader)
        {
            shader = _shader;
        }

        public void Render(Dictionary<TexturedModel, List<Entity>> entities)
        {
            foreach (TexturedModel model in entities.Keys)
            {
                entities.TryGetValue(model, out List<Entity> batch);
                BindCommon(model);
                foreach (var entity in batch)
                {
                    PrepareInstance(entity);
                    GL.DrawElements(BeginMode.Triangles, model.rawModel.VertexCount, DrawElementsType.UnsignedInt, 0);
                }
                UnbindCommon();
            }
        }
        public void BindCommon(TexturedModel texturedModel)
        {
            if (texturedModel.hasTransparency)
                MasterRenderer.DisableCulling();

            RawModel model = texturedModel.rawModel;

            GL.BindVertexArray(model.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            shader.LoadUseFakeLightingVariable(texturedModel.useFakeLighting);
            shader.LoadShineVariables(texturedModel.shineDamper, texturedModel.reflectivity, texturedModel.luminosity);
            shader.LoadEnviromentMap(1);

            texturedModel.BindTexture(TextureUnit.Texture0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, SkyboxRenderer.SkyboxTextureID);
        }
        public void UnbindCommon() 
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.BindVertexArray(0);

            MasterRenderer.EnableCulling();
        }
        public void PrepareInstance(Entity entity)
        {
            shader.LoadTransformationMatrix(entity.gameObject.transform.CalculateTransformationMatrix());
            shader.LoadColour(entity.gameObject.colour.ToVector());
        }

        private readonly EntityShader shader;
    }
}
