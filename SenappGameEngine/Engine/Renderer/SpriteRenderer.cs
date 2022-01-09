using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.UI;

namespace Senapp.Engine.Renderer
{
    public class SpriteRenderer
    {
        private SpriteShader shader;
        private RawModel UIQuad;
        public SpriteRenderer(SpriteShader _shader)
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            shader = _shader;
            shader.Start();
            shader.Stop();
        }

        public void Render(Dictionary<Texture, List<GameObject>> sprites, Vector3 cameraPosition)
        {
            foreach (Texture texture in sprites.Keys)
            {   
                PrepareSprite(texture);
                sprites.TryGetValue(texture, out List<GameObject> batch);
                foreach (GameObject element in batch)
                {
                    var sprite = element.GetComponent<Sprite>();
                    BindSprite(sprite);
                    PrepareInstance(sprite, cameraPosition);
                    GL.DrawElements(BeginMode.Triangles, UIQuad.vertexCount, DrawElementsType.UnsignedInt, 0);
                    UnbindSprite();
                }
            }
        }
        public void PrepareSprite(Texture texture)
        {
            texture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
        public void BindSprite(Sprite element)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(element.quad.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }
        public void UnbindSprite()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest); 
        }
        public void PrepareInstance(Sprite element, Vector3 cameraPosition)
        {
            shader.LoadTransformationMatrix(new Transform(element.GetUIPosition(), element.gameObject.transform.rotation, element.gameObject.transform.localScale).TransformationMatrixUI(cameraPosition));
            shader.LoadColour(element.gameObject.colour);
        }
    }
}
