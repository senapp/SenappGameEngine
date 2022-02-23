using OpenTK;

using Senapp.Engine.Core.Components;
using Senapp.Engine.Utilities;

namespace Senapp.Engine.Entities
{
    public class Camera : Component
    {
        public const float Sensitivity = 0.1f;
        public const float NearPlane = 0.1f;
        public const float FarPlane = 10000f;
        public const float MinFov = 0.1f;
        public const float MaxFov = 120f;

        public float AspectRatio { private get; set; }

        public float Fov => MathHelper.RadiansToDegrees(fov);

        public Camera() {}
        public Camera(float aspectRatio, float fov)
        {
            AspectRatio = aspectRatio;
            SetFov(fov);
        }

        public void SetFov(float value)
        {
            var angle = MathHelper.Clamp(value, MinFov, MaxFov);
            fov = MathHelper.DegreesToRadians(angle);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(gameObject.transform.GetWorldPosition(), gameObject.transform.GetWorldPosition() + gameObject.transform.Front.DegreesToRadians(), gameObject.transform.Up.DegreesToRadians());
        }
        public Matrix4 GetViewMatrixUI()
        {
            return Matrix4.LookAt(gameObject.transform.GetWorldPosition(), gameObject.transform.GetWorldPosition() - Vector3.UnitZ, Vector3.UnitY);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, AspectRatio, NearPlane, FarPlane);
        }
        public Matrix4 GetProjectionMatrixUI()
        {
            return Matrix4.CreatePerspectiveFieldOfView(1.5708f, AspectRatio, NearPlane, FarPlane);
        }

        private float fov = MathHelper.PiOver2;
    }

}


