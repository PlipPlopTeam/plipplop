using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
    public static Vector3 GetHorizontalVelocity (this Rigidbody rigidbody)
    {
        return new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
    }

    public static bool IsPointInTriangle(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var A = 1f / 2f * (-p1.y * p2.x + p0.y * (-p1.x + p2.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y);
        var sign = A < 0 ? -1 : 1;
        var s = (p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y) * sign;
        var t = (p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y) * sign;

        return s > 0 && t > 0 && (s + t) < 2 * A * sign;
    }

    public static Vector3 GetRandomPointAround(float radius)
    {
        float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
        return new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    public static Rect Scale(this Rect rect, float scale)
    {
        return new Rect(
            new Vector2(rect.x*scale, rect.y*scale),
            new Vector2(rect.width*scale, rect.height*scale)
        );
    }

    public static Rect Shift(this Rect rect, Vector2 shift)
    {
        return new Rect(
            new Vector2(rect.x + shift.x, rect.y + shift.y),
            new Vector2(rect.width, rect.height)
        );
    }
}
