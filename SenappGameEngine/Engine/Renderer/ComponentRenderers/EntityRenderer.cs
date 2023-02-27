using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Models;
using Senapp.Engine.Utilities;
using Senapp.Engine.Entities;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.Utilities.Testing;

namespace Senapp.Engine.Renderer.ComponentRenderers
{
    public class EntityRenderer
    {
        public EntityRenderer()
        {
            shader = new EntityShader();
        }

        public void Render(bool isMultisample, Camera camera, Dictionary<TexturedModel, List<Entity>> entities)
        {
            shader.Start();
            shader.LoadCameraMatrix(camera);
            shader.LoadIsMultisample(isMultisample);
            var wireFrame = WireFrame.IsEnabled();
            if (wireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
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
            if (wireFrame) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            shader.Stop();
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
            shader.LoadInstanceId(entity.gameObject.InstanceId);
            shader.LoadInstanceId(entity.gameObject.InstanceId);
        }
        public void Dispose()
        {
            shader.Dispose();
        }

        private readonly EntityShader shader;
    }
}
