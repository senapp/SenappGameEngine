using OpenTK;

using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;

namespace Senapp.Engine.UI
{
    public class Sprite : UIElement
    {
        public Texture texture { get; set; }
        public RawModel quad { get; set; }

        public Sprite() { }
        public Sprite(RawModel quad, Texture texture)
        {
            this.quad = quad;
            this.texture = texture;
        }
        public Sprite(string textureName) :  this(Loader.LoadTexture(textureName)) { }
        public Sprite(Texture texture)
        {
            quad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            this.texture = texture;
        }

        public void BindTexture()
        {
            texture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
    }
}
