using System;

namespace Senapp.Engine.Utilities
{
    public class Randomize
    {
        public static int Range(int min, int max) => RangeInt(min, max);
        public static float Range(float min, float max) => RangeFloat(min, max);
        public static double Range(double min, double max) => RangeDouble(min, max);

        public static int RangeInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public static float RangeFloat(float min, float max)
        {
            Random random = new Random();
            var multipler = (float)random.NextDouble();
            var value = multipler * (max + - min);
            return value + min;
        }
        public static double RangeDouble(double min, double max)
        {
            Random random = new Random();
            var multipler = random.NextDouble();
            var value = multipler * (max + -min);
            return value + min;
        }
    }
}
