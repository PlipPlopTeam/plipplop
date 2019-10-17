using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Volume))]
public class StaticCameraVolume : MonoBehaviour
{
    public Transform aim;
    bool isInside = false;

    // Start is called before the first frame update
    void Start()
    {
        var col = GetComponent<CollisionEventTransmitter>();

        col.onTriggerEnter += Col_onTriggerEnter;
        col.onTriggerExit += Col_onTriggerExit;
    }



    private void Col_onTriggerExit(Collider obj)
    {
        var c = obj.gameObject.GetComponent<Controller>();
        if (isInside && c && Game.i.player.IsPossessing(c))
        {
            //if (previousCamera) Game.i.aperture.SwitchCamera(previousCamera);
            //previousCamera = camera;
            isInside = false;
            Game.i.aperture.Unfreeze();
        }
    }

    private void Col_onTriggerEnter(Collider obj)
    {
        var c = obj.gameObject.GetComponent<Controller>();
        if (!isInside && c && Game.i.player.IsPossessing(c))
        {

            isInside = true;
            Game.i.aperture.Freeze();
            Game.i.aperture.cam.transform.position = aim.position;
            Game.i.aperture.cam.transform.forward = aim.forward;
            //previousCamera = Camera.main;
            //Game.i.aperture.SwitchCamera(camera);
        }
    }
}
