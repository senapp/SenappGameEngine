using OpenTK;
using OpenTK.Graphics;

namespace Senapp.Engine
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (Toolkit.Init())
            {
                GraphicsMode mode = new GraphicsMode(new ColorFormat(24), 16, 8, 4, new ColorFormat(32), 2, false);
                new TestGame(mode);
            }
        }   
    }
}
