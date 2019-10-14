using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSensor : MonoBehaviour
{
    CollisionEventTransmitter collisionEventTransmitter;
    List<Controller> controllers = new List<Controller>();

    private void Awake()
    {
        collisionEventTransmitter = GetComponent<CollisionEventTransmitter>();
        collisionEventTransmitter.onTriggerEnter += OnControllerEnter;
        collisionEventTransmitter.onTriggerExit += OnControllerExit;
    }

    private void OnControllerEnter(Collider obj)
    {
        var ctrl = obj.GetComponent<Controller>();
        if (ctrl) controllers.Add(ctrl);
    }

    private void OnControllerExit(Collider obj)
    {
        var ctrl = obj.GetComponent<Controller>();
        if (ctrl) controllers.RemoveAll(o => o == ctrl);
    }

    public bool IsThereAnyController()
    {
        return controllers.Count > 0;
    }

    public Controller GetFocusedController()
    {
        Controller focused = null;
        float bestMatch = Mathf.Infinity;

        foreach (var c in controllers) {
            var position = Game.i.aperture.cam.WorldToScreenPoint(c.transform.position);
            var dist = Vector3.Distance(position, new Vector3(Screen.width, Screen.height, 0f));
            if (dist < bestMatch) {
                bestMatch = dist;
                focused = c;
            }
        }

        return focused;
    }
}
