using OpenTK;
using OpenTK.Graphics.OpenGL;
using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.UI;
using System;
using System.Collections.Generic;

namespace Senapp.Engine.Renderer
{
    public class UIRenderer
    {
        private UIShader shader;
        private RawModel UIQuad;
        public UIRenderer(UIShader _shader)
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad));
            shader = _shader;
            shader.Start();
            shader.Stop();
        }

        public void Render(Dictionary<Texture, List<GameObject>> UIElements, Vector3 cameraPosition)
        {
            foreach (Texture texture in UIElements.Keys)
            {   
                PrepareSprite(texture);
                UIElements.TryGetValue(texture, out List<GameObject> batch);
                foreach (GameObject element in batch)
                {
                    var elementComponent = element.GetComponent<UIElement>();
                    BindUIElement(elementComponent);
                    var val = new Transform(element.transform.GetUIPosition(), element.transform.rotation, element.transform.localScale);
                    PrepareInstance(elementComponent, val, cameraPosition);
                    GL.DrawElements(BeginMode.Triangles, UIQuad.vertexCount, DrawElementsType.UnsignedInt, 0);
                    UnbindUIElement();
                }
            }
        }
        public void PrepareSprite(Texture texture)
        {
            texture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
        public void BindUIElement(UIElement element)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(element.quad.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }
        public void UnbindUIElement()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest); 
        }
        public void PrepareInstance(UIElement element, Transform transform, Vector3 cameraPosition)
        {
            shader.LoadTransformationMatrix(transform.TransformationMatrixUI(cameraPosition));
            shader.LoadColour(element.sprite.colour);
        }
    }
}
