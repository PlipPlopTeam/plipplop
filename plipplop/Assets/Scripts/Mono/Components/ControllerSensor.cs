using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ControllerSensor : MonoBehaviour
{
    [Range(1, 10)] public float sensorRadius = 5f;
    [Range(0, 5)] public float sensorForwardPosition = 3f;

    CollisionEventTransmitter collisionEventTransmitter;
    List<Controller> controllers = new List<Controller>();

    private void Awake()
    {
        collisionEventTransmitter = GetComponent<CollisionEventTransmitter>();
        GetComponent<SphereCollider>().radius = sensorRadius;
        collisionEventTransmitter.onTriggerEnter += OnControllerEnter;
        collisionEventTransmitter.onTriggerExit += OnControllerExit;
    }

    private void OnControllerEnter(Collider obj)
    {
        var ctrl = obj.GetComponent<Controller>();
        if (ctrl/* && NothingBetween(obj.gameObject)*/) controllers.Add(ctrl);
    }

    private void OnControllerExit(Collider obj)
    {
        var ctrl = obj.GetComponent<Controller>();
        if(ctrl) controllers.RemoveAll(o => o == ctrl);
    }

    public bool NothingBetween(GameObject obj)
    {
        float dis = Vector3.Distance(transform.position, obj.transform.position);
        Vector3 dir = (obj.transform.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dis);

        foreach(RaycastHit h in hits)
        {
            if(h.collider.gameObject != gameObject
            && h.collider.gameObject != obj)
            {
                return true;
            }
        }
        return false;
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
            try {
                var position = Game.i.aperture.currentCamera.WorldToScreenPoint(c.transform.position);
                var dist = Vector3.Distance(position, new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                if (dist < bestMatch) {
                    bestMatch = dist;
                    focused = c;
                }
            }
            catch (MissingReferenceException) {
                Debug.LogError("A controller was destroyed while at possessing range, which should never happen. Check the level streaming chunks and make sure neighbors are properly configured");
            }
        }

        return focused;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, sensorRadius);
    }
}
