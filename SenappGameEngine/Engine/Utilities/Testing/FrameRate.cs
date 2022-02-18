using System;

namespace Senapp.Engine.Utilities.Testing
{
    public static class FrameRate
    {
        public static int FPS => lastFPS;

        public static void Update()
        {
            if (Environment.TickCount - lastTick >= 1000)
            {
                lastFPS = framesPerSecond;
                framesPerSecond = 0;
                lastTick = Environment.TickCount;
            }
            framesPerSecond++;
        }

        private static int lastTick;
        private static int lastFPS;
        private static int framesPerSecond;
    }
}
