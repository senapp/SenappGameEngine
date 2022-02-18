using System.Drawing;
using System.Text;

namespace Senapp.Engine.Utilities
{
    public class Resources
    {
        public static Bitmap GetImage(string fileName)
        {
            return (Bitmap)Properties.Resources.ResourceManager.GetObject(fileName);
        }

        public static Icon GetIcon(string fileName)
        {
            return (Icon)Properties.Resources.ResourceManager.GetObject(fileName);
        }

        public static string GetFile(string fileName)
        {
            try
            {
                return (string)Properties.Resources.ResourceManager.GetObject(fileName);
            }
            catch
            {
                return Encoding.Default.GetString((byte[])Properties.Resources.ResourceManager.GetObject(fileName));
            }
        }
    }
}
