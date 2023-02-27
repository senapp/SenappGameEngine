using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;
using Senapp.Engine.Utilities;
using Senapp.Programs;

namespace Senapp
{
    public class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetProcessDPIAware();

        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            GraphicsSettings.Initialize();

            using (Toolkit.Init())
            {              
                GraphicsMode mode = new(
                    color: new ColorFormat(GraphicsSettings.ColourBits),
                    depth: GraphicsSettings.DepthBits,
                    stencil: GraphicsSettings.StencilBits,
                    // Samples just costs performance because it is not applied to the quad rendered with frame buffers.
                    samples: 0,
                    accum: new ColorFormat(GraphicsSettings.AccumBits),
                    buffers: 2,
                    stereo: false);

                // Place your inherited class of 'Game' here to start your game.
                _ = new MobaGame(mode);
            }
        }   
    }
}
