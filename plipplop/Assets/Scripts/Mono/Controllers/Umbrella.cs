
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Umbrella : Controller
{
    public override void OnEject()
    {
        return;
    }

    public override void OnPossess()
    { 
        return;
    }

    internal override void SpecificMove(Vector3 direction)
    {
        base.Move(direction);
    }

    internal override void Start()
    {
        base.Start();
    }

    internal override void Update()
    {
        base.Update();
    }
}
