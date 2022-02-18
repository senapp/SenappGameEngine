using System;
using System.Linq;
using OpenTK;

using Senapp.Engine.Core.GameObjects;
using Senapp.Engine.PlayerInput;
using Senapp.Engine.UI.Components;
using Senapp.Engine.UI.Components.Abstractions;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Core.Transforms
{
    public class Transform
    {
        public const float UIScalingDivisor = 100;

        public GameObject gameObject;

        public Vector3 LocalPosition { get; private set; }
        public Vector3 LocalRotation { get; private set; }
        public Vector3 LocalScale { get; private set; }

        #region Setting Values
        public void SetNewParent(GameObject parent)
        {
            parentPosition = parent.Parent != null ? parent.transform.parentPosition + parent.transform.GetRealLocalPosition() : parent.transform.GetRealLocalPosition();
            parentRotation = parent.Parent != null ? parent.transform.parentRotation + parent.transform.LocalRotation : parent.transform.LocalRotation;
            parentScale = parent.Parent != null ? parent.transform.parentScale * parent.transform.LocalScale : parent.transform.LocalScale;

            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentPosition = parentPosition + GetRealLocalPosition();
                child.transform.parentRotation = parentRotation + LocalRotation;
                child.transform.parentPosition = parentScale * LocalScale;

                child.transform.Translate(Vector3.Zero);
                child.transform.Rotate(Vector3.Zero);
                child.transform.Scale(Vector3.Zero);
            }
        }
        public void SetPosition(Vector3 positon)
        {
            LocalPosition = positon;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentPosition = parentPosition + GetRealLocalPosition();
                child.transform.Translate(Vector3.Zero);
            }
        }
        public void SetRotation(Vector3 rotation)
        {
            LocalRotation = rotation;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentRotation = parentRotation + LocalRotation;
                child.transform.Rotate(Vector3.Zero);
            }
        }
        public void SetScale(Vector3 scale)
        {
            LocalScale = scale;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentScale = parentScale * LocalScale;
                child.transform.Scale(Vector3.Zero);
            }
        }
        #endregion

        #region Constructor
        public Transform(GameObject gameObject): this(gameObject, Vector3.Zero, Vector3.Zero, Vector3.One) { }
        public Transform(GameObject gameObject, float positionX, float positionY, float positionZ) : this(gameObject, new Vector3(positionX,positionY,positionZ), Vector3.Zero, Vector3.One) { }
        public Transform(GameObject gameObject, float positionX, float positionY, float positionZ, float rotationFactor, float scaleFactor) : this(gameObject, new Vector3(positionX, positionY, positionZ), new Vector3(rotationFactor), new Vector3(scaleFactor)) { }
        public Transform(GameObject gameObject, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ) : this(gameObject, new Vector3(positionX, positionY, positionZ), new Vector3(rotationX, rotationY, rotationZ), Vector3.One) { }
        public Transform(GameObject gameObject, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float scaleX, float scaleY, float scaleZ) : this(gameObject, new Vector3(positionX, positionY, positionZ), new Vector3(rotationX, rotationY,rotationZ), new Vector3(scaleX,scaleY,scaleZ)) { }
        public Transform(GameObject gameObject, Vector3 localPosition, Vector3 localRotation, Vector3 localScale)
        {
            this.LocalPosition = localPosition;
            this.LocalRotation = localRotation;
            this.LocalScale = localScale;
            this.gameObject = gameObject;

            if (gameObject.Parent != null)
            {
                parentPosition = gameObject.Parent.transform.parentPosition + gameObject.Parent.transform.GetRealLocalPosition();
                parentRotation = gameObject.Parent.transform.parentRotation + gameObject.Parent.transform.LocalRotation;
                parentScale = gameObject.Parent.transform.parentScale * gameObject.Parent.transform.LocalScale;
            }
        }
        #endregion

        #region Transforming
        public void Translate(Vector3 position)
        {
            LocalPosition += position;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentPosition = parentPosition + GetRealLocalPosition();
                child.transform.Translate(Vector3.Zero);
            }
        }
        public void Translate(float x, float y, float z) => Translate(new Vector3(x, y, z));
        public void Translate(float x, float y) => Translate(new Vector3(x, y, 0));
        public void Translate(float translationFactor) => Translate(new Vector3(translationFactor, translationFactor, translationFactor));
        
        public void Rotate(Vector3 rotation)
        {
            LocalRotation += rotation;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentRotation = parentRotation + LocalRotation;
                child.transform.Rotate(Vector3.Zero);
            }
        }
        public void Rotate(float rotation) => Rotate(new Vector3(rotation, 0, 0));
        public void Rotate(float x, float y) => Rotate(new Vector3(x, y, 0));
        public void Rotate(float x, float y, float z) => Rotate(new Vector3(x, y, z));

        public void Scale(Vector3 scaleFactor)
        {
            LocalScale += scaleFactor;
            gameObject.IsGameObjectUpdated = true;

            foreach (var child in gameObject.Children.Values)
            {
                child.transform.parentScale = parentScale * LocalScale;
                child.transform.Scale(Vector3.Zero);
            }
        }
        public void Scale(float scaleFactor) => Scale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
        public void Scale(float x, float y) => Scale(new Vector3(x, y, 0));
        public void Scale(float x, float y, float z) => Scale(new Vector3(x, y, z));
        #endregion

        #region World Dimensions
        public static Matrix4 DefaultTransformationMatrixTranslation()
        {
            Matrix4 translation = Matrix4.CreateTranslation(Vector3.Zero);
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(Vector3.Zero));
            Matrix4 scale = Matrix4.CreateScale(Vector3.One);
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }
        public Matrix4 TransformationMatrixTranslation()
        {
            return Matrix4.CreateTranslation(new Vector3(GetWorldPosition()));
        }
        public Matrix4 CalculateTransformationMatrix() => CalculateTransformationMatrix(Vector3.Zero, null, null);
        public Matrix4 CalculateTransformationMatrix(Vector3 cameraPosition, Text text, Sprite sprite)
        {
            var wPosition = cameraPosition + GetWorldPosition();
            var wRotation = GetWorldRotation();
            var wScale = GetWorldScale();

            wPosition = gameObject.IsGameObjectUI 
                ? wPosition.WithZ(cameraPosition.Z - 1) 
                : wPosition;

            wRotation = gameObject.IsGameObjectUI
                ? wRotation.WithX(0) 
                : wRotation;

            wScale = text != null 
                ? LocalScale 
                : sprite != null 
                    ? new Vector3(wScale.X * sprite.size.X, wScale.Y * sprite.size.Y, wScale.Z) 
                    : wScale;

            Matrix4 translation = Matrix4.CreateTranslation(wPosition);
            Matrix4 rota = Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(wRotation.DegreesToRadiansVector()));
            Matrix4 scale = Matrix4.CreateScale(wScale);
            Matrix4 transformationMatrix = Matrix4.Mult(rota, Matrix4.Mult(scale, translation));
            return transformationMatrix;
        }

        public Vector3 GetWorldVerticePosition(Vector3 vertice)
        {
            var wPositon = GetWorldPosition();
            var wScale = GetWorldScale();

            float x = wPositon.X + vertice.X * wScale.X;
            float y = wPositon.Y + vertice.Y * wScale.Y;
            float z = wPositon.Z + vertice.Z * wScale.Z;
            return new Vector3(x, y, z);
        }
        public Vector3 GetVerticeLength(Vector3 vertice)
        {
            var wScale = GetWorldScale();

            float x = vertice.X * wScale.X;
            float y = vertice.Y * wScale.Y;
            float z = vertice.Z * wScale.Z;
            return new Vector3(x, y, z);
        }
        #endregion

        #region Local Dimensions
        public Vector3 Right => GetRightVector();
        public Vector3 Up => GetUpVector();
        public Vector3 Front => GetFrontVector();

        private Vector3 GetRightVector()
        {
            return Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        }
        private Vector3 GetUpVector()
        {
            return Vector3.Normalize(Vector3.Cross(Right, Front));
        }
        private Vector3 GetFrontVector()
        {
            var _front = -Vector3.UnitZ;

            _front.X = (float)Math.Cos(MathHelper.DegreesToRadians(LocalRotation.X)) * (float)Math.Cos(MathHelper.DegreesToRadians(270-LocalRotation.Y));
            _front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(LocalRotation.X));
            _front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(LocalRotation.X)) * (float)Math.Sin(MathHelper.DegreesToRadians(270 - LocalRotation.Y));

            return Vector3.Normalize(_front);
        }
        #endregion

        #region Extra
        public Vector3 SetPositionFromMouse(Vector3 offset)
        {
            var mouseInput = Input.GetMousePositionWindowCenter();
            return LocalPosition = new Vector3(mouseInput.X / Game.Instance.Width * 200 * Game.Instance.AspectRatio + offset.X - parentPosition.X, -mouseInput.Y / Game.Instance.Height * 200 + offset.Y - parentPosition.Y, LocalPosition.Z + offset.Z - parentPosition.Z);
        }
        public Vector3 LookAt(Vector3 target)
        {
            var Rad2Deg = 57.29578f;

            var wPosition = GetWorldPosition();

            float yX = target.X - wPosition.X;
            float yY = target.Z - wPosition.Z;
            double newAngleY = Math.Atan2(yY, yX);
            newAngleY *= Rad2Deg;

            return new Vector3(LocalRotation.X, 180 - (float)newAngleY, LocalRotation.Z);
        }

        /// <summary>
        /// Rotate transformation towards target
        /// Blend: No change on call = 0 - 1 = Instantly rotate to target
        /// </summary>
        public void RotateTowardsTarget(Vector3 target, float blend, float rotationOffset = 0)
        {
            var targetRotation = LookAt(target);
            var currentRotation = LocalRotation;
            // rotation.Y > 360 & rotation.Y < 0 solution
            if (currentRotation.Y + rotationOffset - targetRotation.Y > 180)
            {
                LocalRotation = new Vector3(currentRotation.X, currentRotation.Y - 360, currentRotation.Z);
            }
            else if (currentRotation.Y + rotationOffset - targetRotation.Y < -180)
            {
                LocalRotation = new Vector3(currentRotation.X, currentRotation.Y + 360, currentRotation.Z);
            }

            LocalRotation = Vector3.Lerp(LocalRotation, new Vector3(targetRotation.X, targetRotation.Y - rotationOffset, targetRotation.Z), blend);
        }

        public Vector3 GetWorldPosition()
        {
            var customPositon = GetRealLocalPosition();
            if (gameObject?.Parent != null)
            {
                var returnPosition = parentPosition + customPositon;
                return gameObject.IsGameObjectUI ? returnPosition / UIScalingDivisor : returnPosition;
            }
            else
            {
                return customPositon;
            }
        }
        public Vector3 GetWorldRotation()
        {
            if (gameObject?.Parent != null)
            {
                var returnRotation = parentRotation + LocalRotation;
                return returnRotation;
            }
            else
            {
                return LocalRotation;
            }
        }
        public Vector3 GetWorldScale()
        {
            if (gameObject?.Parent != null)
            {
                var returnScale = parentScale * LocalScale;
                return returnScale;
            }
            else
            {
                return LocalScale;
            }
        }

        public Vector3 GetRealLocalPosition()
        {
            if (gameObject.IsGameObjectUI)
            {
                return gameObject.ComponentManager.GetComponents().Values.Cast<IComponentUI>().FirstOrDefault().GetUIPosition();
            }
            else
            {
                return LocalPosition;
            }
        }
        #endregion

        private Vector3 parentPosition;
        private Vector3 parentRotation;
        private Vector3 parentScale;
    }
}
