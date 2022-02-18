using OpenTK.Graphics.OpenGL;

namespace Senapp.Engine.Utilities.Testing
{
    public static class WireFrame
    {
        public static bool IsEnabled() => enabled;
        public static void Enable(bool mode)
        {
            enabled = mode;

            if (enabled)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
        }

        private static bool enabled = false;
    }
}
