using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Volume))]
public class StaticCameraVolume : MonoBehaviour
{
    new Camera camera;
    Camera previousCamera;
    bool isInside = false;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        var col = GetComponent<CollisionEventTransmitter>();

        col.onTriggerEnter += Col_onTriggerEnter;
        col.onTriggerExit += Col_onTriggerExit;
    }

    private void Col_onTriggerExit(Collider obj)
    {
        var c = obj.gameObject.GetComponent<Controller>();
        if (isInside && c && Game.i.player.IsPossessing(c)) {
            if (previousCamera) Game.i.aperture.SwitchCamera(previousCamera);
            previousCamera = camera;
            isInside = false;
        }
    }

    private void Col_onTriggerEnter(Collider obj)
    {
        var c = obj.gameObject.GetComponent<Controller>();
        if (!isInside && c && Game.i.player.IsPossessing(c)) {
            isInside = true;
            previousCamera = Camera.main;
            Game.i.aperture.SwitchCamera(camera);
        }
    }
}
