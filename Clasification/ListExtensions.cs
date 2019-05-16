using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification
{
    public static class ListExtensions
    {
        private static Random random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int listSize = list.Count;
            while (listSize > 1)
            {
                listSize--;
                int randomIndex = random.Next(listSize + 1);
                T value = list[randomIndex];
                list[randomIndex] = list[listSize];
                list[listSize] = value;
            }
        }
    }
}
