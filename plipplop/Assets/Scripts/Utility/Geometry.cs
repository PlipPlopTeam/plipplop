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
}
