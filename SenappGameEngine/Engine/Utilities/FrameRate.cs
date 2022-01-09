using System;

namespace Senapp.Engine.Utilities
{
    public class FrameRate
    {
        private static int lastTick;
        private static int lastFPS;
        private static int FPS;
        private static bool FPSEnabled = true;

        public static void Enable(bool mode)
        {
            FPSEnabled = mode;
        }
        public static bool IsEnabled()
        {
            return FPSEnabled;
        }
        public static int Get()
        {
            return lastFPS;
        }
        public static void Update()
        {
            if (FPSEnabled)
            {
                if (Environment.TickCount - lastTick >= 1000)
                {
                    lastFPS = FPS;
                    FPS = 0;
                    lastTick = Environment.TickCount;
                }
                FPS++;
            }
        }
    }
}
