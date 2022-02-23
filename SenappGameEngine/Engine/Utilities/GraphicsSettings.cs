using System;

using Senapp.Engine.Core;

namespace Senapp.Engine.Utilities
{
    public class GraphicsSettings
    {
        public static void Initialize()
        {
            ColourBits = Settings.GetSetting<int>(ConfigSettings.COLOUR_BITS);
            DepthBits = Settings.GetSetting<int>(ConfigSettings.DEPTH_BITS);
            StencilBits = Settings.GetSetting<int>(ConfigSettings.STENCIL_BITS);
            AccumBits = Settings.GetSetting<int>(ConfigSettings.ACCUM_BITS);
        }

        public static bool RestartRequired { get; private set; }

        public static int ColourBits { get; private set; }
        public static bool SetColourBits(int bits)
        {
            var success = Settings.SetSetting(ConfigSettings.COLOUR_BITS, ColourBits = bits);
            if (success)
            {
                RestartRequired = true;
                return true;
            }

            return false;
        }

        public static int DepthBits { get; private set; }
        public static bool SetDepthBits(int bits)
        {
            var success = Settings.SetSetting(ConfigSettings.DEPTH_BITS, DepthBits = bits);
            if (success)
            {
                RestartRequired = true;
                return true;
            }

            return false;
        }

        public static int StencilBits { get; private set; }
        public static bool SetStenciBits(int bits)
        {
            var success = Settings.SetSetting(ConfigSettings.STENCIL_BITS, StencilBits = bits);
            if (success)
            {
                RestartRequired = true;
                return true;
            }

            return false;
        }

        public static int AccumBits { get; private set; }
        public static bool SetAccumBits(int bits)
        {
            var success = Settings.SetSetting(ConfigSettings.ACCUM_BITS, AccumBits = bits);
            if (success)
            {
                RestartRequired = true;
                return true;
            }

            return false;
        }
    }
}
