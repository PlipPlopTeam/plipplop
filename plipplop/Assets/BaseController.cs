using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseController : Controller
{
    public override void OnEject()
    {
        base.OnEject();
        gameObject.SetActive(false);
    }

    public override void OnPossess()
    {
        base.OnPossess();
        gameObject.SetActive(true);
    }

    internal override void SpecificMove(Vector3 direction)
    {

    }

    internal override void Start()
    {
        base.Start();
        // Code here
    }

    internal override void Update()
    {
        base.Update();
        // Code here
    }

    internal override void OnLegsRetracted()
    {
        // Code here
    }

    internal override void OnTryPossess()
    {
        if (controllerSensor && controllerSensor.IsThereAnyController())
        {
            Game.i.player.Possess(controllerSensor.GetFocusedController());
        }
    }

    internal override void OnLegsExtended()
    {
        // Code here
    }
}
