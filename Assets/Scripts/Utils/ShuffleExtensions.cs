using System;

namespace System.Collections.Generic
{
    public static class ShuffleExtensions
    {
        public static void Shuffle<T>(this T[] array, int seed)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            Shuffle(array, new Random(seed));
        }

        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            Shuffle(list, new Random(seed));
        }

        public static void Shuffle<T>(this T[] array, Random random)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (random == null) throw new ArgumentNullException(nameof(random));

            for (int i = array.Length; i > 1; i--)
            {
                int j = random.Next(i);
                if (j == i - 1) continue;
                (array[j], array[i - 1]) = (array[i - 1], array[j]);
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (random == null) throw new ArgumentNullException(nameof(random));

            for (int i = list.Count; i > 1; i--)
            {
                int j = random.Next(i);
                if (j == i - 1) continue;
                (list[j], list[i - 1]) = (list[i - 1], list[j]);
            }
        }
    }
}
