using OpenTK;
using Senapp.Engine.UI;
using System;
using Senapp.Engine.Utilities;

namespace Senapp.Engine
{
    public enum UIPosition { Center, Top, Bottom, Left, Right, TopLeft, TopRight, CenterLeft, CenterRight, BottomLeft, BottomRight }
    public class Transform
    {
        public static readonly float UIScalingConst = 10000;
        public static readonly float TextYOffset = 6;

        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 localScale { get; set; }
        private UIPosition UIPos = UIPosition.Center;
        public Vector3 LookAt(Vector3 target)
        {
            var Rad2Deg = 57.29578f;

            float yX = target.X - position.X;
            float yY = target.Z - position.Z;
            double newAngleY = Math.Atan2(yY, yX);
            newAngleY *= Rad2Deg;

            return new Vector3(rotation.X,270 - (float)newAngleY, rotation.Z);
        } 
        public void SetUIPosition(UIPosition pos)
        {
            UIPos = pos;
        }
        public Vector4 GetUIDimensionsPixels(bool isText, Text text = null)
        {
            Vector2 size = new Vector2(Game.Instance.Width, Game.Instance.Height);
            var val = GetUIPosition();
            val = new Vector3(val.X + Game.Instance.AspectRatio - (0.5f * localScale.X), -(val.Y - 1 + 0.5f * localScale.Y), val.Z);
            if (isText) val = new Vector3(val.X, val.Y + TextYOffset * (text.fontSize / UIScalingConst), val.Z);

            var xLength = Game.Instance.AspectRatio * 2;
            var yLength = 2;

            var xVal = val.X / xLength;
            var xMaxVal = (val.X + localScale.X) / xLength;
            var yVal = val.Y / yLength;
            var yMaxVal = (val.Y + localScale.Y) / yLength;

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

            var borderX = -Game.Instance.AspectRatio + (0.5f * localScale.X);
            var borderY = 1 - 0.5f * localScale.Y ;

            switch (UIPos)
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

            return new Vector3(borderX + position.X / (UIScalingConst / 100), borderY + position.Y / (UIScalingConst / 100), z);
        }
        public Matrix4 TransformationMatrix()
        {
            return CalculateTransformationMatrix();
        }
        public Matrix4 TransformationMatrixUI(Vector3 cameraPosition)
        {
            return CalculateTransformationMatrixUI(cameraPosition);
        }

        public static Transform NewWithScale(float x, float y, float z)
        {
            return new Transform(0, 0, 0, 0, 0, 0, x, y, z);
        }
        public static Transform NewWithScale(float x, float y)
        {
            return new Transform(0, 0, 0, 0, 0, 0, x, y, 0);
        }
        public static Transform NewFromRotation(float x, float y, float z)
        {
            return new Transform(0, 0, 0, x, y, z);
        }
        public static Transform NewFromRotation(float x, float y)
        {
            return new Transform(0, 0, 0, x, y, 0);
        }
        public Transform(): this(Vector3.Zero, Vector3.Zero, Vector3.One) { }
        public Transform(float x, float z) : this(x, 0, z) { }
        public Transform(float x , float y, float z) : this(new Vector3(x,y,z), Vector3.Zero, Vector3.One) { }
        public Transform(float x, float y, float z, float rotationFactor, float scaleFactor) : this(new Vector3(x, y, z), new Vector3(rotationFactor), new Vector3(scaleFactor)) { }
        public Transform(float xPosition, float yPosition, float zPosition, float xRotation, float yRotation, float zRotation) : this(new Vector3(xPosition, yPosition, zPosition), new Vector3(xRotation, yRotation, zRotation), Vector3.One) { }
        public Transform(float xPosition, float yPosition, float zPosition, float xRotation, float yRotation, float zRotation, float xScale, float yScale, float zScale) : this(new Vector3(xPosition, yPosition, zPosition), new Vector3(xRotation, yRotation,zRotation), new Vector3(xScale,yScale,zScale)) { }

        public Transform(Vector3 position, Vector3 rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
        public void Translate(Vector3 position)
        {
            this.position += position;
        }
        public void Translate(float x, float y, float z)
        {
            Translate(new Vector3(x, y, z));
        }
        public void Translate(float x, float y)
        {
            Translate(new Vector3(x, y, 0));
        }
        public void Translate(float translationFactor)
        {
            Translate(new Vector3(translationFactor, translationFactor, translationFactor));
        }
        public void Rotate(float rotation)
        {
            Rotate(new Vector3(rotation, 0,0));
        }
        public void Rotate(float x, float y)
        {
            Rotate(new Vector3(x, y, 0));
        }
        public void Rotate(float x, float y, float z)
        {
            Rotate(new Vector3(x,y,z));
        }
        public void Rotate(Vector3 rotation)
        {
            this.rotation += rotation;
        }
        public void Scale(float scaleFactor)
        {
            Scale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
        }
        public void Scale(float x, float y)
        {
            Scale(new Vector3(x, y, 1));
        }
        public void Scale(float x, float y, float z)
        {
            Scale(new Vector3(x, y, z));
        }
        public void Scale(Vector3 scaleFactor)
        {
            localScale += scaleFactor;
        }
        public Matrix4 TransformationMatrixTranslation()
        {
            return Matrix4.CreateTranslation(new Vector3(position.X, position.Y, position.Z));
        }
        private Matrix4 CalculateTransformationMatrix()
        {
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(position.X, position.Y, position.Z));
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(rotation.X), MathHelper.DegreesToRadians(rotation.Y), MathHelper.DegreesToRadians(rotation.Z)));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(localScale.X, localScale.Y, localScale.Z));
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }
        private Matrix4 CalculateTransformationMatrixUI(Vector3 cameraPosition)
        {  
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(cameraPosition.X+position.X,cameraPosition.Y + position.Y, cameraPosition.Z + position.Z));
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(rotation.X), MathHelper.DegreesToRadians(rotation.Y), MathHelper.DegreesToRadians(rotation.Z)));
            Matrix4 scale = Matrix4.CreateScale(new Vector3(localScale.X, localScale.Y, localScale.Z));
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }
        public Vector3 GetVerticePosition(Vector3 vertice)
        {
            float x = position.X + vertice.X * localScale.X;
            float y = position.Y + vertice.Y * localScale.Y;
            float z = position.Z + vertice.Z * localScale.Z;
            return new Vector3(x, y, z);
        }
        public Vector3 GetVerticeLength(Vector3 vertice)
        {
            float x = vertice.X * localScale.X;
            float y = vertice.Y * localScale.Y;
            float z = vertice.Z * localScale.Z;
            return new Vector3(x, y, z);
        }
        public Vector3 Right => _right();
        public Vector3 Up => _up();
        public Vector3 Front => _front();
        private Vector3 _right()
        {
            return Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        }
        private Vector3 _up()
        {
            return Vector3.Normalize(Vector3.Cross(Right, Front));
        }
        private Vector3 _front()
        {
            var _front = -Vector3.UnitZ;

            _front.X = (float)Math.Cos(MathHelper.DegreesToRadians(rotation.X)) * (float)Math.Cos(MathHelper.DegreesToRadians(270-rotation.Y));
            _front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(rotation.X));
            _front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(rotation.X)) * (float)Math.Sin(MathHelper.DegreesToRadians(270 - rotation.Y));

            return Vector3.Normalize(_front);
        }
    }
}
