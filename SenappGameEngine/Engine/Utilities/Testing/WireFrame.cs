using OpenTK.Graphics.OpenGL;

namespace Senapp.Engine.Utilities.Testing
{
    public static class WireFrame
    {
        public static bool IsEnabled() => enabled;
        public static void Enable(bool mode)
        {
            enabled = mode;
        }

        private static bool enabled = false;
    }
}
