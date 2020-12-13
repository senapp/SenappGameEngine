using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Senapp.Engine.Models;
using Senapp.Engine.Entities;
using System.Collections.Generic;
using Senapp.Engine.Shaders;
using System;
using Senapp.Engine.Base;

namespace Senapp.Engine.Renderer
{
    public class EntityRenderer
    {
        private EntityShader shader;
        public EntityRenderer(EntityShader _shader)
        {
            shader = _shader;
        }

        public void Render(Dictionary<TexturedModel, List<GameObject>> entities)
        {
            foreach (TexturedModel model in entities.Keys)
            {
                PrepareTexturedModel(model);
                entities.TryGetValue(model,out List<GameObject> batch);
                foreach (GameObject entity in batch)
                {
                    PrepareInstance(entity);
                    GL.DrawElements(BeginMode.Triangles, model.rawModel.vertexCount, DrawElementsType.UnsignedInt, 0);
                }
                UnbindTexturedModel();
            }

        }
        public void PrepareTexturedModel(TexturedModel texturedModel)
        {
           if (texturedModel.hasTransparency)
                MasterRenderer.DisableCulling();
            RawModel model = texturedModel.rawModel;
            GL.BindVertexArray(model.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            shader.LoadUseFakeLightingVariable(texturedModel.useFakeLighting);
            shader.LoadShineVariables(texturedModel.shineDamper, texturedModel.reflectivity, texturedModel.luminosity);
            texturedModel.BindTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);

        }
        public void UnbindTexturedModel() 
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.BindVertexArray(0);
            MasterRenderer.EnableCulling();
        }
        public void PrepareInstance(GameObject entity)
        {
            shader.LoadTransformationMatrix(entity.transform.TransformationMatrix());
        }
    }
}
