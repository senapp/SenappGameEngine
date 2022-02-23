namespace Senapp.Engine.Utilities
{
    public class Mathematics
    {
        public static double UniqueCombine(double a, double b)
        {
            return 0.5f * (a + b) * (a + b + 1) + b;
        }

        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
    }
}
