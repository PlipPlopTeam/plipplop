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

        Vector2 rot = new Vector2(
            Input.GetAxis("Mouse Y") + mapping.Axis(ACTION.CAMERA_VERTICAL),
            Input.GetAxis("Mouse X") + mapping.Axis(ACTION.CAMERA_HORIZONTAL)
        );
        Game.i.aperture.Rotate(rot.x * 2f, rot.y * 2f);
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
        var wasCrouching = false;
        if (this.controller != null) {
            wasCrouching = this.controller.AreLegsRetracted();
            Eject();
        }
        controller.OnPossess();
        this.controller = controller;
        Game.i.aperture.target = controller.transform;
        Game.i.aperture.settings = controller.customCamera ? controller.customCamera.settings : Game.i.aperture.defaultSet.settings;
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
