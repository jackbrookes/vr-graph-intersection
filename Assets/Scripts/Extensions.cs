using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{

    /// <summary>
    /// Random number generator with seed based on current time.
    /// </summary>
    /// <returns></returns>
    private static System.Random rng = new System.Random();

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
    {
        int i = 0;
        foreach (var item in items)
        {
            if (count == 1)
                yield return new T[] { item };
            else
            {
                foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                    yield return new T[] { item }.Concat(result);
            }

            ++i;
        }
    }


    public static T[] RemoveAt<T>(this T[] source, int index)
    {
        T[] dest = new T[source.Length - 1];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }

    public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
    {
        return listToClone.GetRange(0, listToClone.Count);
    }


    /// <summary>
    /// Shuffles a list in-place with a given random number generator.
    /// </summary>
    /// <param name="list">List to shuffle</param>
    /// <param name="rng">Random number generator via which the shuffling occurs</param>
    public static void Shuffle<T>(this IList<T> list, System.Random rng)
    {

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Shuffles a list in-place with the current time based random number generator. 
    /// </summary>
    /// <param name="list">List to shuffle</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        list.Shuffle(rng);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T element in source)
        {
            action(element);
        }
    }

}
