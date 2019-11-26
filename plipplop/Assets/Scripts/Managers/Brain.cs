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
        Possess(baseController);
    }

    public void TeleportBaseControllerAndPossess()
    {
        Vector3 dir = (controller.transform.position - Game.i.aperture.position.current).normalized;
        dir = new Vector3(dir.x, 0f, dir.z);
        baseController.transform.position = controller.transform.position + dir * 2f;
        // TODO : Faire un raycast pour empecher les speedrunners de passer à travers le mur
        Possess(baseController);
    }

    public void Update()
    {
        UpdateController();
        Game.i.aperture.RotateWithGamepad(
            mapping.Axis(EAction.CAMERA_VERTICAL),
            mapping.Axis(EAction.CAMERA_HORIZONTAL)
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
            mapping.Axis(EAction.MOVE_FORWARD_BACK),
            mapping.Axis(EAction.MOVE_RIGHT_LEFT)
        );

        controller.MoveCamera(
            mapping.Axis(EAction.CAMERA_HORIZONTAL),
            mapping.Axis(EAction.CAMERA_VERTICAL)
        );
    }

    public void UpdateController()
    {
        if (controller is null) return;

        if (mapping.IsPressed(EAction.JUMP)) controller.OnJump();
        if (mapping.IsPressed(EAction.POSSESS)) controller.OnTryPossess();
        if (mapping.IsHeld(EAction.JUMP)) controller.OnHoldJump();
    }

    public void Possess(Controller controller)
    {
        if (this.controller != null) {
            Eject();
        }
        controller.OnPossess();
        this.controller = controller;
        Game.i.aperture.target = controller.transform;
        Game.i.aperture.Load(controller.customCamera ?? Game.i.library.defaultAperture);
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

    public bool IsPossessingBaseController()
    {
        return controller == baseController;
    }
}
