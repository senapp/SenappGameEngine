using System;

using Senapp.Engine.Core;

namespace Senapp.Engine.Utilities
{
    public enum AntiAliasingTypes
    {
        NONE,
        FXAA, 
        MSAA,
    }

    public class GraphicsSettings
    {
        public static void Initialize()
        {
            AntiAliasing = Settings.GetSetting<AntiAliasingTypes>(ConfigSettings.ANTI_ALIASING);
            StartupAA = AntiAliasing;

            FXAASamples = Settings.GetSetting<int>(ConfigSettings.FXAA_SAMPLES);
            MSAASamples = Settings.GetSetting<int>(ConfigSettings.MSAA_SAMPLES);

            ColourBits = Settings.GetSetting<int>(ConfigSettings.COLOUR_BITS);
            DepthBits = Settings.GetSetting<int>(ConfigSettings.DEPTH_BITS);
            StencilBits = Settings.GetSetting<int>(ConfigSettings.STENCIL_BITS);
            AccumBits = Settings.GetSetting<int>(ConfigSettings.ACCUM_BITS);
        }

        public static bool RestartRequired { get; private set; }
        public static bool RestartPrefered { get; private set; }
        public static bool PostProcessingRequired => AntiAliasing == AntiAliasingTypes.MSAA;

        public static AntiAliasingTypes AntiAliasing { get; private set; }
        public static bool SetAntiAliasing(AntiAliasingTypes antiAliasing)
        {
            if (StartupAA == AntiAliasingTypes.FXAA && antiAliasing != AntiAliasingTypes.FXAA)
            {
                Console.WriteLine("[GRAPHICS] Orginal AA was 'FXAA', restart prefered for better performance");
                RestartPrefered = true;
            }

            if (StartupAA != AntiAliasingTypes.FXAA && antiAliasing == AntiAliasingTypes.FXAA)
            {
                Console.WriteLine("[GRAPHICS] Orginal AA was not 'FXAA', restart required to apply FXAA samples");
                RestartRequired = true;
            }

            var success = Settings.SetSetting(ConfigSettings.ANTI_ALIASING, AntiAliasing = antiAliasing);
            if (success)
            {
                Game.Instance.Renderer.RecalculateSize(Game.Instance.Width, Game.Instance.Height);
                return true;
            }

            return false;
        }

        public static int FXAASamples { get; private set; }
        public static bool SetFXAA(int samples)
        {
            var success = Settings.SetSetting(ConfigSettings.FXAA_SAMPLES, FXAASamples = samples);
            if (success)
            {
                RestartRequired = true;
                return true;
            }

            return false;
        }

        public static int MSAASamples { get; private set; }
        public static bool SetMSAA(int samples)
        {
            var success = Settings.SetSetting(ConfigSettings.MSAA_SAMPLES, MSAASamples = samples);
            if (success)
            {
                Game.Instance.Renderer.RecalculateSize(Game.Instance.Width, Game.Instance.Height);
                return true;
            }

            return false;
        }

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

        private static AntiAliasingTypes StartupAA;
    }
}
