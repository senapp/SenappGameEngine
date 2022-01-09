using OpenTK;

using Senapp.Engine.Base;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Entities
{
    public class Camera : Component
    {
        public static readonly float Sensitivity = 0.1f;

        private float _fov = MathHelper.PiOver2;
        private float nearPlane = 0.1f;
        private float farPlane = 10000f;
        private float minFov = 0.1f;
        private float maxFov = 120f;

        public Camera()
        {

        }
        public Camera(float aspectRatio,float fov)
        {
            AspectRatio = aspectRatio;
            Fov = fov;
        }

        public float AspectRatio { private get; set; }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, minFov, maxFov);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(gameObject.transform.position, gameObject.transform.position + gameObject.transform.Front.DegreesToRadians(), gameObject.transform.Up.DegreesToRadians());
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, nearPlane, farPlane);
        }
        public Matrix4 GetViewMatrixUI()
        {
            return Matrix4.LookAt(gameObject.transform.position, gameObject.transform.position - Vector3.UnitZ, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrixUI()
        {
            return Matrix4.CreatePerspectiveFieldOfView(1.5708f, AspectRatio, nearPlane, farPlane);
        }
    }

}


