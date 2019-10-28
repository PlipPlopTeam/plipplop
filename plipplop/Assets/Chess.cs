using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : Vanilla
{
    internal override void Update()
    {
        base.Update();

        Vector3 rigidDirection = rigidbody.velocity.normalized;
        Vector3 direction = new Vector3(rigidDirection.x, 0f, rigidDirection.z);

        //visuals.forward = direction;
        //visuals.transform.Rotate(direction, Time.deltaTime * 10f);
    }
}
