using OpenTK.Graphics.OpenGL4;

namespace Senapp.Engine.Utilities
{
    public static class WireFrame
    {
        private static bool enabled = false;
        public static bool IsEnabled()
        {
            return enabled;
        }
        public static void Enable(bool mode)
        {
            enabled = mode;
        }
    }
}
