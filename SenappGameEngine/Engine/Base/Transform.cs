using System;

using OpenTK;

using Senapp.Engine.UI;

namespace Senapp.Engine.Base
{
    public enum UIPosition { Center, Top, Bottom, Left, Right, TopLeft, TopRight, CenterLeft, CenterRight, BottomLeft, BottomRight }

    public class Transform
    {
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 localScale { get; set; }

        #region Constructor
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
        #endregion

        #region Initalizing
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
        #endregion

        #region Transforming
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
        #endregion

        #region World Dimensions
        public Matrix4 TransformationMatrix()
        {
            return CalculateTransformationMatrix();
        }
        public Matrix4 TransformationMatrixUI(Vector3 cameraPosition)
        {
            return CalculateTransformationMatrixUI(cameraPosition);
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
        #endregion

        #region Local Dimensions
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
        #endregion

        #region Extra
        public Vector3 LookAt(Vector3 target)
        {
            var Rad2Deg = 57.29578f;

            float yX = target.X - position.X;
            float yY = target.Z - position.Z;
            double newAngleY = Math.Atan2(yY, yX);
            newAngleY *= Rad2Deg;

            return new Vector3(rotation.X, 180 - (float)newAngleY, rotation.Z);
        }

        /// <summary>
        /// Rotate transformation towards target
        /// Blend: No change on call = 0 <-> 1 = Instantly rotate to target
        /// </summary>
        public void RotateTowardsTarget(Vector3 target, float blend, float rotationOffset = 0)
        {
            var targetRotation = LookAt(target);
            var currentRotation = rotation;
            // rotation.Y > 360 & rotation.Y < 0 solution
            if (currentRotation.Y + rotationOffset - targetRotation.Y > 180)
            {
                rotation = new Vector3(currentRotation.X, currentRotation.Y - 360, currentRotation.Z);
            }
            else if (currentRotation.Y + rotationOffset - targetRotation.Y < -180)
            {
                rotation = new Vector3(currentRotation.X, currentRotation.Y + 360, currentRotation.Z);
            }

            rotation = Vector3.Lerp(rotation, new Vector3(targetRotation.X, targetRotation.Y - rotationOffset, targetRotation.Z), blend);
        }
        #endregion
    }
}
