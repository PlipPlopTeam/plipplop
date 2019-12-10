using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Tools
{
    public static Vector3 Mean(this Vector3[] vectors)
    {
        var sum = Vector3.zero;
        foreach(var v in vectors) {
            sum += v;
        }
        sum /= vectors.Length;

        return sum;
    }

    public static void AddUnique<T>(this List<T> list, T element)
    {
        if (typeof(T) is object) {
            list.RemoveAll(o => ReferenceEquals(o, element));
        }
        else {
            list.RemoveAll(o => !EqualityComparer<T>.Default.Equals(element, o));
        }
        list.Add(element);
    }

    public static T PickRandom<T>(this IEnumerable<T> list)
    {
        if (list.Count() <= 0) {
            throw new System.Exception("The provided array is of zero size.");
        }
        return list.ElementAt(Random.Range(0, list.Count()));
    }

    public static string Letter(this int value)
    {
        string c = char.ConvertFromUtf32(65 + value);
        return c;
    }

    public static string Format(this string str, params object[] elements)
    {
        return string.Format(str, elements);
    }
}
