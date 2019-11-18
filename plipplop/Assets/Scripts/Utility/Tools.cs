using System.Collections;
using System.Collections.Generic;
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
}
