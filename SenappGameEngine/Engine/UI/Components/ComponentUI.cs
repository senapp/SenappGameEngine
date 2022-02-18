using System.Linq;

using OpenTK;

using Senapp.Engine.Core;
using Senapp.Engine.Core.Components;
using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.Core.Transforms;
using Senapp.Engine.UI.Components.Abstractions;

namespace Senapp.Engine.UI.Components
{
    public enum UIPosition { 
        Center, 
        Top, 
        Bottom, 
        Left, 
        Right, 
        TopLeft, 
        TopRight, 
        CenterLeft, 
        CenterRight, 
        BottomLeft, 
        BottomRight 
    }

    public class ComponentUI : Component, IComponentUI
    {
        public const float TextOffset = 50;
        public int SortingLayer = 0;
        public UIPosition UIConstriant = UIPosition.Center;

        protected Vector4 dimensions = Vector4.Zero;
        protected Vector3 lastPosition = Vector3.Zero;
        protected Vector3 lastScale = Vector3.Zero;

        public override bool ComponentConditions(GameObject gameObject)
        {
            return gameObject.IsGameObjectUI && !gameObject.ComponentManager.GetComponents().Keys.Any(type => type.IsSubclassOf(typeof(ComponentUI)));
        }

        public virtual Vector4 GetUIDimensionsPixels()
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

            position = new Vector3(position.X + Game.Instance.AspectRatio - (0.5f * scale.X), -(position.Y - 1 + 0.5f * scale.Y), position.Z);

            var xLength = Game.Instance.AspectRatio * 2;
            var yLength = 2;

            var xVal = position.X / xLength;
            var xMaxVal = (position.X + scale.X) / xLength;
            var yVal = position.Y / yLength;
            var yMaxVal = (position.Y + scale.Y) / yLength;

            return dimensions = new Vector4((int)(screenSize.X * xVal), (int)(screenSize.Y * yVal), (int)(screenSize.X * xMaxVal), (int)(screenSize.Y * yMaxVal));
        }
        public virtual Vector3 GetUIPosition()
        {
            var z = -1;

            var scale = gameObject.transform.GetWorldScale();

            var borderX = (-Game.Instance.AspectRatio + (0.5f * scale.X)) * Transform.UIScalingDivisor;
            var borderY = (1 - 0.5f * scale.Y) * Transform.UIScalingDivisor;

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
    }
}
