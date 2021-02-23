using System;
using System.Collections.Generic;
using System.Text;

namespace Senapp.Engine.Utilities
{
    public class Mathematics
    {
        public static double UniqueCombine(double a, double b)
        {
            return 0.5f * (a + b) * (a + b + 1) + b;
        }
    }
}
