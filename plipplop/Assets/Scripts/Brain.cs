using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    Controller controller = null;
    Mapping mapping;

    public Brain(Mapping mapping)
    {
        this.mapping = mapping;
    }

    public void Update()
    {
        UpdateController();
    }

    public void UpdateController()
    {
        if (controller is null) return;

        if (mapping.IsPressed(ACTION.POSSESS)) controller.OnEject();
        if (mapping.IsPressed(ACTION.JUMP)) controller.OnJump();
        controller.Move(
            mapping.Axis(ACTION.MOVE_FORWARD_BACK),
            mapping.Axis(ACTION.MOVE_RIGHT_LEFT)
        );
    }

    public void Possess(Controller controller)
    {
        this.controller = controller;
    }
}
