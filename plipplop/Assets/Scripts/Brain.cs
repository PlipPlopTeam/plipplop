using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    Controller controller = null;
    Controller baseController = null;
    Mapping mapping;

    public Brain(Mapping mapping)
    {
        this.mapping = mapping;
    }

    public Controller GetCurrentController()
    {
        return controller;
    }

    public void SetBaseController(Controller c)
    {
        baseController = c;
    }

    public void PossessBaseController()
    {
        Vector3 dir = (controller.transform.position - Game.i.aperture.position.current).normalized;
        dir = new Vector3(-dir.x, 0f, -dir.z);
        baseController.transform.position = controller.transform.position + dir * 5f;
        Possess(baseController);
    }

    public void Update()
    {
        UpdateController();
        Game.i.aperture.RotateWithGamepad(
            mapping.Axis(ACTION.CAMERA_VERTICAL),
            mapping.Axis(ACTION.CAMERA_HORIZONTAL)
        );
    }

    public void FixedUpdate()
    {
        UpdateControllerPhysics();
    }

    public void UpdateControllerPhysics()
    {
        if (controller is null) return;

        controller.Move(
            mapping.Axis(ACTION.MOVE_FORWARD_BACK),
            mapping.Axis(ACTION.MOVE_RIGHT_LEFT)
        );
    }

    public void UpdateController()
    {
        if (controller is null) return;

        if (mapping.IsPressed(ACTION.JUMP)) controller.OnJump();
        if (mapping.IsPressed(ACTION.POSSESS)) controller.OnTryPossess();
        if (mapping.IsHeld(ACTION.JUMP)) controller.OnHoldJump();
    }

    public void Possess(Controller controller)
    {
        if (this.controller != null) {
            Eject();
        }
        controller.OnPossess();
        this.controller = controller;
        Game.i.aperture.target = controller.transform;
        Game.i.aperture.Load(controller.customCamera ? controller.customCamera.settings : Game.i.library.defaultAperture.settings);
    }

    public void Eject()
    {
        this.controller.OnEject();
        controller = null;
    }

    public bool IsPossessing(Controller controller)
    {
        return controller == this.controller;
    }
}
