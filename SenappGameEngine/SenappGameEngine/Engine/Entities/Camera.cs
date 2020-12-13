using System;
using OpenTK;
using Senapp.Engine.Shaders;
using Senapp.Engine.Base;

namespace Senapp.Engine.Entities
{
    public class Camera : Component
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        private float _pitch;
        private float _yaw = -MathHelper.PiOver2; 
        private float _fov = MathHelper.PiOver2;
        private float nearPlane = 0.1f;
        private float farPlane = 10000f;
        private float minFov = 0.1f;
        private float maxFov = 120f;
        public float Sensitivity = 0.2f;



        public Camera()
        {

        }
        public Camera(float aspectRatio,float fov)
        {
            AspectRatio = aspectRatio;
            Fov = fov;
        }

        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, minFov, maxFov);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix(Transform transform)
        {
            return Matrix4.LookAt(transform.position, transform.position + _front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, nearPlane, farPlane);
        }
        public Matrix4 GetViewMatrixUI(Transform transform)
        {
            return Matrix4.LookAt(transform.position, transform.position - Vector3.UnitZ, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrixUI()
        {
            return Matrix4.CreatePerspectiveFieldOfView(1.5708f, AspectRatio, nearPlane, farPlane);
        }

        private void UpdateVectors()
        {
            _front.X = (float)Math.Cos(_pitch) * (float)Math.Cos(_yaw);
            _front.Y = (float)Math.Sin(_pitch);
            _front.Z = (float)Math.Cos(_pitch) * (float)Math.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }

}


