using OpenTK;

using Senapp.Engine.Base;

namespace Senapp.Engine.UI
{
    public class UIElement : Component
    {
        public static readonly float UIScalingConst = 10000;
        public static readonly float TextYOffset = 6;

        public int SortingLayer = 0;
        public UIPosition UIConstriant { get; set; } = UIPosition.Center;

        public Vector4 GetUIDimensionsPixels(bool isText, Text text = null)
        {
            Vector2 size = new Vector2(Game.Instance.Width, Game.Instance.Height);
            var val = GetUIPosition();
            val = new Vector3(val.X + Game.Instance.AspectRatio - (0.5f * gameObject.transform.localScale.X), -(val.Y - 1 + 0.5f * gameObject.transform.localScale.Y), val.Z);
            if (isText) val = new Vector3(val.X, val.Y + TextYOffset * (text.fontSize / UIScalingConst), val.Z);

            var xLength = Game.Instance.AspectRatio * 2;
            var yLength = 2;

            var xVal = val.X / xLength;
            var xMaxVal = (val.X + gameObject.transform.localScale.X) / xLength;
            var yVal = val.Y / yLength;
            var yMaxVal = (val.Y + gameObject.transform.localScale.Y) / yLength;

            if (isText)
            {
                var xTextLength = (int)(size.X * (text.textLength * (text.fontSize / UIScalingConst) / xLength));
                var yTextLength = (int)(size.Y * ((text.textHeight) * (text.fontSize / UIScalingConst) / yLength));
                return new Vector4((int)(size.X * xVal), (int)(size.Y * yVal), (int)(size.X * xVal + xTextLength), (int)(size.Y * yVal + yTextLength));
            }

            return new Vector4((int)(size.X * xVal), (int)(size.Y * yVal), (int)(size.X * xMaxVal), (int)(size.Y * yMaxVal));
        }

        public Vector3 GetUIPosition()
        {
            var z = -1;

            var borderX = -Game.Instance.AspectRatio + (0.5f * gameObject.transform.localScale.X);
            var borderY = 1 - 0.5f * gameObject.transform.localScale.Y;

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

            return new Vector3(borderX + gameObject.transform.position.X / (UIScalingConst / 100), borderY + gameObject.transform.position.Y / (UIScalingConst / 100), z);
        }
    }
}
