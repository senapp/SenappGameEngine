using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.UI;

namespace Senapp.Engine.Renderer
{
    public class TextRenderer
    {
        private TextShader shader;
        private RawModel UIQuad;
        public TextRenderer(TextShader _shader)
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            shader = _shader;
            shader.Start();
            shader.Stop();
        }

        public void Render(Dictionary<GameFont, List<GameObject>> texts, Vector3 cameraPosition)
        {
            foreach (GameFont font in texts.Keys)
            {
                PrepareTexture(font.fontAtlas);
                texts.TryGetValue(font, out List<GameObject> batch);
                foreach (GameObject text in batch)
                {
                    var textComponent = text.GetComponent<Text>();
                    for (int i = 0; i < textComponent.textCharactersID.Count; i++)
                    {
                        var character = textComponent.textCharactersCustomID[i];

                        if (font.characterRawModels.TryGetValue(character, out RawModel model))
                        {
                            BindCharacter(model);
                        }

                        var rawPos = textComponent.GetUIPosition();
                        var val = new Transform(new Vector3(rawPos.X + textComponent.textRenderLengths[i] * textComponent.fontSize / UIElement.UIScalingConst, rawPos.Y - textComponent.textRenderHeightsOffsets[i] * textComponent.fontSize / UIElement.UIScalingConst, rawPos.Z), Vector3.Zero, Vector3.One);
                        
                        PrepareInstance(text, val, cameraPosition);
                        GL.DrawElements(BeginMode.Triangles, UIQuad.vertexCount, DrawElementsType.UnsignedInt, 0);
                        UnbindCharacter();
                    }
                }
            }
        }
        public void PrepareTexture(Texture texture)
        {
            texture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
        public void BindCharacter(RawModel character)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(character.vaoID);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
        }
        public void UnbindCharacter()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
        }
        public void PrepareInstance(GameObject text, Transform transform, Vector3 cameraPosition)
        {
            shader.LoadTransformationMatrix(transform.TransformationMatrixUI(cameraPosition));
            shader.LoadColour(text.colour);
        }
    }
}