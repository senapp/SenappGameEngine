using OpenTK;
using Senapp.Engine.Core;
using Senapp.Engine.Core.Transforms;
using static Senapp.Engine.Utilities.VectorExtensions;

namespace Senapp.Engine.Raycasts
{
    public class Raycast
    {
        public static Vector3 ClosestPoint(Vector2 mouseLocation, float pointY)
        {
            var modelView = Transform.DefaultTransformationMatrixTranslation() * Game.Instance.MainCamera.GetViewMatrix();
            var projection = Game.Instance.MainCamera.GetProjectionMatrix();
            Vector3 near = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 0), modelView, projection); // start of ray (near plane)
            Vector3 far = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 1), modelView, projection); // end of ray (far plane)

            return PointOnLineY(near, far, pointY);
        }

        private static Vector3 UnProject(Vector3 screen, Matrix4 modelView, Matrix4 projection)
        {
            int[] viewport = new int[4];
            OpenTK.Graphics.OpenGL.GL.GetInteger(OpenTK.Graphics.OpenGL.GetPName.Viewport, viewport);

            Vector4 pos = new();

            pos.X = (screen.X - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            pos.Y = 1 - (screen.Y - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            pos.Z = screen.Z * 2.0f - 1.0f;
            pos.W = 1.0f;

            Vector4 pos2 = Vector4.Transform(pos, Matrix4.Invert(projection) * Matrix4.Invert(modelView));
            Vector3 pos_out = new(pos2.X, pos2.Y, pos2.Z);

            return pos_out / pos2.W;
        }
    }
}
