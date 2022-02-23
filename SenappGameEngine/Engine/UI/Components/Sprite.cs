using OpenTK;
using OpenTK.Graphics.OpenGL;

using Senapp.Engine.Core;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.Loaders;
using Senapp.Engine.Models;
using Senapp.Engine.Renderer.Helper;

namespace Senapp.Engine.UI.Components
{
    public class Sprite : ComponentUI
    {
        public Texture texture;
        public RawModel quad;
        public Vector2 size = Vector2.One;

        public Sprite() : this(string.Empty) { }
        public Sprite(string textureName)
        {
            quad = Loader.LoadToVAO(Geometry.GetVertex(Geometries.Quad), Geometry.GetVertexName(Geometries.Quad));
            texture = Loader.LoadTexture(textureName);
        }

        public override Vector4 GetUIDimensionsPixels()
        {
            if (gameObject.IsGameObjectUpdated)
            {
                gameObject.IsGameObjectUpdated = false;
            }
            else
            {
                return dimensions;
            }

            Vector2 screenSize = new(Game.Instance.Width, Game.Instance.Height);
            var position = gameObject.transform.GetWorldPosition();
            var scale = gameObject.transform.GetWorldScale();

            position = new Vector3(position.X + Game.Instance.AspectRatio - (0.5f * scale.X * size.X), -(position.Y - 1 + 0.5f * scale.Y * size.Y), position.Z);

            var xLength = Game.Instance.AspectRatio * 2;
            var yLength = 2;

            var xVal = position.X / xLength;
            var xMaxVal = (position.X + scale.X * size.X) / xLength;
            var yVal = position.Y / yLength;
            var yMaxVal = (position.Y + scale.Y * size.Y) / yLength;

            return dimensions = new Vector4((int)(screenSize.X * xVal), (int)(screenSize.Y * yVal), (int)(screenSize.X * xMaxVal), (int)(screenSize.Y * yMaxVal));
        }
        public override Vector3 GetUIPosition()
        {
            var z = -1;

            var scale = gameObject.transform.GetWorldScale();

            var borderX = (-Game.Instance.AspectRatio + (0.5f * scale.X * size.X)) * Transform.UIScalingDivisor;
            var borderY = (1 - 0.5f * scale.Y * size.Y) * Transform.UIScalingDivisor;

            switch (UIConstriant)
            {
                case UIPosition.Top:
                    borderX = 0;
                    break;
                case UIPosition.Bottom:
                    borderY = -borderY;
                    borderX = 0;
                    break;
                case UIPosition.Left:
                    borderY = 0;
                    break;
                case UIPosition.Right:
                    borderX = -borderX;
                    borderY = 0;
                    break;
                case UIPosition.Center:
                    borderX = 0;
                    borderY = 0;
                    break;
                case UIPosition.TopRight:
                    borderX = -borderX;
                    break;
                case UIPosition.CenterLeft:
                    borderY = 0;
                    break;
                case UIPosition.CenterRight:
                    borderY = 0;
                    borderX = -borderX;
                    break;
                case UIPosition.BottomLeft:
                    borderY = -borderY;
                    break;
                case UIPosition.BottomRight:
                    borderY = -borderY;
                    borderX = -borderX;
                    break;
                default:
                    break;
            }

            return new Vector3(borderX + gameObject.transform.LocalPosition.X, borderY + gameObject.transform.LocalPosition.Y, z);
        }

        public void BindTexture(TextureUnit texture)
        {
            this.texture.Bind(texture);
        }

        public Sprite WithUIConstraint(UIPosition constraint)
        {
            this.UIConstriant = constraint;
            return this;
        }
        public Sprite WithSortingLayer(int sortingLayer)
        {
            this.SortingLayer = sortingLayer;
            return this;
        }
        public Sprite WithSize(Vector2 size)
        {
            this.size = size;
            return this;
        }
    }
}
