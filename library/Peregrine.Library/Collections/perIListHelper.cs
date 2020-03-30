using System;
using System.Collections.Generic;

namespace Peregrine.Library.Collections
{
    public static class perIListHelper
    {
        public static void Shuffle<T>( this IList<T> list)
        {
            list.Shuffle(0);
        }

        public static void Shuffle<T>(this IList<T> list, int randomKey)
        {
            if (randomKey == 0)
            {
                var now = DateTime.Now;
                var i1 = now.DateToInt();
                var i2 = now.TimeToInt();
                randomKey = i1 ^ i2;
            }

            var random = new Random(randomKey);

            list.Shuffle(random);
        }

        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            var n = list.Count;

            while (n > 1)
            {
                var k = random.Next(n--);
                list.Swap(n, k);
            }
        }

        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            if (a == b || a < 0 || b < 0 || a >= list.Count || b >= list.Count)
            {
                return;
            }

            var temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }

        public static IReadOnlyCollection<T> Backwards<T>(this IList<T> list)
        {
            var result = new List<T>(list.Count);

            for (var i = list.Count - 1; i >= 0; i++)
            {
                result.Add(list[i]);
            }

            return result.AsReadOnly();
        }
    }
}
