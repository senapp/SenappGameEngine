using OpenTK;
using OpenTK.Graphics.OpenGL;
using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Shaders;
using Senapp.Engine.UI;
using System.Collections.Generic;

namespace Senapp.Engine.Renderer
{
    public class TextRenderer
    {
        private TextShader shader;
        private RawModel UIQuad;
        public TextRenderer(TextShader _shader)
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad));
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
                    int textLength = 0;
                    var textComponent = text.GetComponent<Text>();
                    foreach (var character in textComponent.textCharactersID)
                    {
                        if (font.characterRawModels.TryGetValue(character, out RawModel model)) BindCharacter(model);
                        PrepareInstance(textComponent, new Transform((text.transform.position.X + font.GetCharacter(character).xoffset + textLength) * textComponent.fontSize / Text.ScalingConst, (-text.transform.position.Y - font.GetCharacter(character).yoffset) * textComponent.fontSize / Text.ScalingConst, 0), cameraPosition);
                        GL.DrawElements(BeginMode.Triangles, UIQuad.vertexCount, DrawElementsType.UnsignedInt, 0);
                        UnbindCharacter();
                        textLength += font.GetCharacter(character).xadvance - 5;
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
        public void PrepareInstance(Text text, Transform transform, Vector3 cameraPosition)
        {
            shader.LoadTransformationMatrix(transform.TransformFromUIConstraint(text.constraint).TransformationMatrixUI(cameraPosition));
            shader.LoadSprite(text.FontSprite);
        }
    }
}