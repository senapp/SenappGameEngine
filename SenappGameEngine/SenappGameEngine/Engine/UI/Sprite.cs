using OpenTK;
using Senapp.Engine.Models;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Senapp.Engine.UI
{
    public class Sprite
    {
        public Texture texture { get; set; }
        public Vector3 colour = Vector3.One;

        public Sprite(Texture tex) 
        {
            texture = tex;
        }
        public Sprite(string tex)
        {
            texture = Loader.LoadTexture(tex);
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
