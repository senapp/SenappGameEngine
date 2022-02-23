using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.UI;
using Senapp.Engine.UI.Components;
using Senapp.Engine.Utilities;
using Senapp.Engine.Renderer.Helper;
using Senapp.Engine.Core;
using Senapp.Engine.Shaders.Components;
using Senapp.Engine.Entities;

namespace Senapp.Engine.Renderer.ComponentRenderers
{   
    public class TextRenderer
    {
        public TextRenderer()
        {
            UIQuad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            shader = new TextShader();
            shader.Start();
            shader.Stop();
        }

        public void Start(Camera camera)
        {
            shader.Start();
            shader.LoadCameraMatrix(camera);
        }
        public void Stop()
        {
            shader.Stop();
        }
        public void Render(Dictionary<GameFont, List<Text>> texts)
        {
            foreach (var font in texts.Keys)
            {
                BindCommon(font);
                texts.TryGetValue(font, out List<Text> textsBatch);

                Dictionary<double, List<TextRenderObject>> characters = new();

                foreach (var text in textsBatch)
                {
                    var colour = text.gameObject.colour.ToVector();
                    var rawPos = text.gameObject.transform.GetWorldPosition() * Transform.UIScalingDivisor;
                    var cameraPosition = Game.Instance.MainCamera.gameObject.transform.GetWorldPosition();
                    var rawMatrix = text.gameObject.transform.CalculateTransformationMatrix(cameraPosition, text, null);

                    for (int i = 0; i < text.TextCharactersId.Count; i++)
                    {
                        var translation = new Vector4(
                                cameraPosition.X + (rawPos.X + text.TextRenderLengths[i] * text.RawFontSize * Transform.UIScalingDivisor) / Transform.UIScalingDivisor,
                                cameraPosition.Y + (rawPos.Y - text.TextRenderHeightsOffsets[i] * text.RawFontSize * Transform.UIScalingDivisor - ComponentUI.TextOffset) / Transform.UIScalingDivisor, 
                                cameraPosition.Z - 1, 1);
                        
                        var newMatrix = new Matrix4(rawMatrix.Row0, rawMatrix.Row1, rawMatrix.Row2, translation);

                        var renderObject = new TextRenderObject(newMatrix, colour);

                        characters.TryGetValue(text.TextCharactersCustomId[i], out List<TextRenderObject> batch);
                        if (batch != null)
                        {
                            batch.Add(renderObject);
                        }
                        else
                        {
                            List<TextRenderObject> newBatch = new();
                            newBatch.Add(renderObject);
                            characters[text.TextCharactersCustomId[i]] = newBatch;
                        }
                    }
                }

                foreach (var id in characters.Keys)
                {
                    if (font.characterRawModels.TryGetValue(id, out RawModel model))
                    {
                        BindCharacter(model);
                    }

                    characters.TryGetValue(id, out List<TextRenderObject> textRenderObjects);
                    foreach (var renderObject in textRenderObjects)
                    {
                        PrepareCharacter(renderObject.colour, renderObject.matrix);
                        GL.DrawElements(BeginMode.Triangles, UIQuad.VertexCount, DrawElementsType.UnsignedInt, 0);
                    }

                    UnbindCharacter();
                }
            }
        }
        public void BindCommon(GameFont font)
        {
            font.fontAtlas.Bind(TextureUnit.Texture0);
        }
        public void BindCharacter(RawModel character)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(character.VaoId);
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
        public void PrepareCharacter(Vector3 colour, Matrix4 matrix)
        {
            shader.LoadTransformationMatrix(matrix);
            shader.LoadColour(colour);
        }
        public void Dispose()
        {
            shader.Dispose();
            UIQuad.Dispose();
        }

        private readonly TextShader shader;
        private readonly RawModel UIQuad;
    }

    internal class TextRenderObject
    {
        public Matrix4 matrix;
        public Vector3 colour;

        public TextRenderObject(Matrix4 matrix, Vector3 colour)
        {
            this.matrix = matrix;
            this.colour = colour;
        }
    }
}