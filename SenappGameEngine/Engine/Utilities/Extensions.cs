using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senapp.Engine.Utilities
{
    public static class Extensions
    {
        public static bool UniqueAdd<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
