using System;
using System.Collections.Generic;
using UniLinq;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

public static class IListExtension
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        // var count = ts.Count;
        // var last = count - 1;
        // for (var i = 0; i < last; ++i)
        // {
        //     var r = MathUtils.RandomInt(i, count);
        //     var tmp = ts[i];
        //     ts[i] = ts[r];
        //     ts[r] = tmp;
        // }
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static void ShuffleRange<T>(this IList<T> ts, int startIndex, int endIndex)
    {
        if (startIndex < 0 || startIndex >= ts.Count || endIndex < 0 || endIndex >= ts.Count || startIndex > endIndex)
        {
            throw new ArgumentException();
        }

        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T RandomElement<T>(this IList<T> list)
    {
        return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
    }

    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        int count = Enumerable.Count(enumerable);
        int index = Random.Range(0, count);
        return count == 0 ? default : Enumerable.ElementAt(enumerable, index);
    }

    public static T Find<T>(this IList<T> list, Predicate<T> predicate)
    {
        foreach (var item in list)
        {
            if (predicate.Invoke(item))
            {
                return item;
            }
        }

        return default;
    }

    public static void CleanUp<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = default(T);
        }
    }

    public static T Last<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }

    public static bool IsSubsetOf<T>(this IList<T> a, IList<T> b)
    {
        return !a.Except(b).Any();
    }

    public static T NextItemClosedLoop<T>(this IList<T> list, int index)
    {
        int convertedIndex = index % list.Count;
        return list[convertedIndex];
    }

    public static IList<T> GetRandomElements<T>(this IList<T> list, int elementCount)
    {
        // return list.OrderBy(arg => Guid.NewGuid()).Take(elementCount).ToList();
        return list.OrderBy(arg => Random.Range(-99999, 99999)).Take(elementCount).ToList();
    }

    public static T Pop<T>(this IList<T> items)
    {
        if (items.Count > 0)
        {
            T temp = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return temp;
        }
        else
            return default(T);
    }

    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T) item.Clone()).ToList();
    }
}