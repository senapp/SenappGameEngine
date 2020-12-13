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
        public UIConstraint constraint { get; set; }
        public UIElement() { }
        public UIElement(RawModel _quad, Sprite _sprite, UIConstraint _constraint)
        {
            quad = _quad;
            sprite = _sprite;
            constraint = _constraint;
        }
        public UIElement(Sprite _sprite, UIConstraint _constraint)
        {
            quad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad));
            sprite = _sprite;
            constraint = _constraint;

        }
        public void BindTexture()
        {
            sprite.BindTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }
    }
    public enum UIConstraint
    {
        None,
        TopLeft,
        Top,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        Bottom,
        BottomRight
    }
}
