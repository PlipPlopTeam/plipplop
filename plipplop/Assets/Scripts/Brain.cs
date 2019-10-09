using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    Controller controller = null;

    public void Update()
    {
        if (controller != null) return;

        //controller.OnEject(Input.GetButton(""));
        //controller.OnJump(Input.GetButton(""));
        //controller.OnToggleCrouch(Input.GetButton(""));
    }
}
