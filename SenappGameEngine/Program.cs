using System;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics;

using Senapp.Programs;

namespace Senapp
{
    public class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            using (Toolkit.Init())
            {
                GraphicsMode mode = new GraphicsMode(
                    color: new ColorFormat(24),
                    depth: 16,
                    stencil: 8, 
                    samples: 8,
                    accum: new ColorFormat(32),
                    buffers: 2,
                    stereo: false);

                var gameStartup = new TestGame(mode);
            }
        }   
    }
}
