using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Senapp.Engine.Utilities
{
    public class Resources
    {

        public static List<string> GetAllImages()
        {
            return Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true).Cast<DictionaryEntry>().Where(x => x.Value.GetType() == typeof(Bitmap)).Select(x => x.Key.ToString()).ToList();
        }
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
            var returnData = string.Empty;
            var data = Properties.Resources.ResourceManager.GetObject(fileName);
            if (data != null)
            {
                if (data.GetType().Equals(typeof(string)))
                {
                    returnData = (string)data;
                }
                else
                {
                    returnData = Encoding.Default.GetString((byte[])data);
                }
            }

            if (data == null || string.IsNullOrEmpty(returnData))
            {
                Console.WriteLine($"[ENGINE][ERROR] Error trying to read file '{fileName}' from resources.");
                return null;
            }

            return returnData;
        }
    }
}
