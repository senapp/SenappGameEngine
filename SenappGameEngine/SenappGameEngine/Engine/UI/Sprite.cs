using OpenTK;
using Senapp.Engine.Models;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Senapp.Engine.UI
{
    public class Sprite
    {
        public Texture texture { get; set; }
        public Vector4 colour { get; set; }
        public Vector4 colourEdge { get; set; }
        public float colourMixRatio { get; set; }
        public float edgeRatio { get; set; }
        public float roundnessRatio { get; set; }


        private Vector4 ConvertColor(Color color)
        {
            return new Vector4(color.R / 256, color.G / 256, color.B / 256, color.A / 256);
        }
        private Vector4 ConvertColor(Vector4 color)
        {
            return new Vector4(color.X / 256, color.Y / 256, color.Z / 256, color.W / 256);
        }
        public Sprite(string texFileName, Vector4 color, Vector4 colorEdge, float colorMixRatio, float edgeratio, float roundnessratio)
        {
            texture = Loader.LoadTexture(texFileName);
            colour = ConvertColor(color);
            colourEdge = ConvertColor(colorEdge);
            colourMixRatio = colorMixRatio;
            edgeRatio = edgeratio;
            roundnessRatio = roundnessratio;
        }
        public Sprite(string texFileName, Color color, Color colorEdge, float colorMixRatio, float edgeratio, float roundnessratio) 
            : this(texFileName, new Vector4(color.R, color.G, color.B, color.A), new Vector4(colorEdge.R, colorEdge.G, colorEdge.B, colorEdge.A), 
                  colorMixRatio, edgeratio, roundnessratio) { }
        public Sprite(string texFileName, Color color) : this(texFileName, color, Color.Black, 1, 0, 0) { }
        public Sprite(string texFileName, Vector4 color) : this(texFileName, color, new Vector4(0,0,0,0), 1, 0, 0) { }
        public Sprite(string texFileName) : this(texFileName, Color.Black, Color.Black, 0, 0, 0) { }
        public Sprite() : this(null, Color.Black, Color.Black, 0, 0, 0) { }
        public Sprite(Texture tex) 
        {
            texture = tex;
            colour = ConvertColor(Color.Black);
            colourEdge = ConvertColor(Color.Black);
            colourMixRatio = 0;
            edgeRatio = 0;
            roundnessRatio = 0;
        }

        public void BindTexture(TextureUnit textureUnit)
        {
            texture.Bind(textureUnit);
        }
        public void Dispose()
        {
            texture.Dispose();
        }
    }
}
