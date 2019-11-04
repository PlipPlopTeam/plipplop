using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraApertureDebugger : MonoBehaviour
{
    public AperturePreset preset;
    public Game gamePrefab;
    public Transform dummyTarget;

    Aperture aperture;

    private void Start()
    {
        if (dummyTarget) Destroy(dummyTarget.gameObject);
    }

#if UNITY_EDITOR
    #region INSPECTOR 
    void OnDrawGizmosSelected()
    {
        var target = dummyTarget;

        if (!EditorApplication.isPlaying) {
            aperture = new Aperture(preset ?? gamePrefab.library.defaultAperture);
            aperture.target = target;

            aperture.Apply();
        }
        else {
            aperture = Game.i.aperture;
            target = aperture.target;
        }

        var settings = aperture.GetSettings();
        var cam = aperture.cam;
        var position = aperture.position;
        var distance = aperture.GetHDistanceToTarget();

        var blue = new Color32(173, 216, 230, 255);
        var red = new Color32(230, 150, 160, 255);

        Gizmos.color = blue;
        Handles.color = Gizmos.color;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = blue;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 12;

        if (target) {
            //Debug.Log(settings);
            // Follow range draw
            var segments = 8;
            
            Handles.DrawWireDisc(target.position, Vector3.up, settings.distance.min);
            Handles.DrawWireDisc(target.position, Vector3.up, settings.distance.max);

            Handles.color = new Color32(blue.r, blue.g, blue.b, 70);
            for (var i = 0; i<segments; i++) {
                var portion = Mathf.Deg2Rad*((360f/segments)*i);
                var x = Mathf.Sin(portion);
                var y = Mathf.Cos(portion);
                Handles.DrawLine(
                    target.position + new Vector3(x * settings.distance.min, 0f, y * settings.distance.min), 
                    target.position + new Vector3(x * settings.distance.max, 0f, y * settings.distance.max)
                );
            }
            Handles.color = blue;
            Handles.Label(target.position + Vector3.right * settings.distance.min, "Min " + settings.distance.min.ToString(), style);
            Handles.Label(target.position + Vector3.right * settings.distance.max, "Max " + settings.distance.max.ToString(), style);
            Handles.Label((position.destination + target.position) / 2, "Dist " + distance.ToString(), style);


            Handles.color = new Color32(red.r, red.g, red.b, 40);
            Handles.DrawSolidDisc(target.position, Vector3.up, settings.absoluteBoundaries.min);
            Handles.color = red;
            Handles.DrawWireDisc(target.position, Vector3.up, settings.absoluteBoundaries.max);

            Gizmos.DrawLine(position.current, position.destination);

            Gizmos.DrawWireSphere(position.destination, 1f);
            Gizmos.DrawWireSphere(position.current, 0.6f);

            Gizmos.color = new Color32(30, 30, 255, 255);
            Gizmos.DrawWireCube(new Vector3(position.current.x, target.position.y, position.current.z), 0.5f * Vector3.one);
            Gizmos.DrawLine(position.current, new Vector3(position.current.x, target.position.y, position.current.z));
            /*
            Gizmos.color = new Color32(255, 130, 130, 255);
            style.normal.textColor = Gizmos.color;

            var upper = new Vector3(0f, Mathf.Sin(settings.rotationClamp.min * Mathf.Deg2Rad), Mathf.Cos(settings.rotationClamp.min * Mathf.Deg2Rad)) * settings.distance.max;
            var lower = new Vector3(0f, Mathf.Sin(settings.rotationClamp.max * Mathf.Deg2Rad), Mathf.Cos(settings.rotationClamp.max * Mathf.Deg2Rad)) * settings.distance.max;
            Handles.Label(upper, settings.rotationClamp.min + "°", style);
            Handles.Label(lower, settings.rotationClamp.max + "°", style);
            Gizmos.DrawLine(position.current, upper);
            Gizmos.DrawLine(position.current, lower);
            Gizmos.DrawLine(position.current, target.position);
            */
        }

        //Gizmos.DrawLine(transform.position,
        //    transform.position + new Vector3( transform.forward.x, 0f, transform.forward.z)
        //);
    }
    #endregion
#endif
}
