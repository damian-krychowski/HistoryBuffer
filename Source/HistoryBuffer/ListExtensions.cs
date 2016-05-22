using System.Collections.Generic;
using System.Linq;

namespace HistoryBuffer
{
    internal static class ListExtensions
    {
        public static int MaxIndex<T>(this List<T> list)
        {
            return list.Count - 1;
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return !list.Any();
        }

        public static void RemoveAllStartingFrom<T>(this List<T> list, int startFromIndex)
        {
            for (var i = list.MaxIndex(); i >= startFromIndex; i--)
            {
                list.RemoveAt(i);
            }
        }
    }
}