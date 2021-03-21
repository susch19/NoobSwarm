using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm
{
    public static class Extension
    {
        public static T? FirstOrNull<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : struct
        {
            foreach (var item in enumerable)
            {
                if (predicate(item))
                    return new T?(item);
            }
            return null;
        }
    }
}
