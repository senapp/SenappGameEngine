using OpenTK;
using Senapp.Engine.Base;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer;
using System;

namespace Senapp.Engine.UI
{

    public class UIElement : Component
    {
        public Sprite sprite { get; set; }
        public RawModel quad { get; set; }
        public UIElement() { }
        public UIElement(RawModel _quad, Sprite _sprite)
        {
            quad = _quad;
            sprite = _sprite;
        }
        public UIElement(Sprite _sprite)
        {
            quad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad));
            sprite = _sprite;

        }
        public void BindTexture()
        {
            sprite.BindTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
    }
}
