using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 GetHorizontalVelocity (this Rigidbody rigidbody)
    {
        return new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
    }
}
