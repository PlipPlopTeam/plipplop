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
        Destroy(dummyTarget.gameObject);
    }

    #region INSPECTOR 
    void OnDrawGizmos()
    {
        var target = dummyTarget;

        if (!EditorApplication.isPlaying) {
            aperture = new Aperture(preset ? preset.settings : gamePrefab.library.defaultAperture.settings);
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

        Gizmos.color = new Color32(173, 216, 230, 255);
        Handles.color = Gizmos.color = new Color32(173, 216, 230, 255);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color32(173, 216, 230, 255);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 12;

        if (target) {
            //Debug.Log(settings);
            // Follow range draw
            Handles.DrawWireDisc(position.destination, Vector3.up, settings.distance.min);
            Handles.DrawWireDisc(position.destination, Vector3.up, settings.distance.max);
            Handles.Label(position.destination + Vector3.right * settings.distance.min, "Min " + settings.distance.min.ToString(), style);
            Handles.Label(position.destination + Vector3.right * settings.distance.max, "Max " + settings.distance.max.ToString(), style);
            Handles.Label((position.destination + target.position) / 2, "Dist " + distance.ToString(), style);

            Gizmos.DrawLine(position.current, position.destination);

            Gizmos.DrawWireSphere(position.destination, 1f);
            Gizmos.DrawWireSphere(position.current, 0.6f);

            Gizmos.color = new Color32(30, 30, 255, 255);
            Gizmos.DrawWireCube(new Vector3(target.position.x, position.current.y, target.position.z), 0.5f * Vector3.one);
            Gizmos.DrawLine(position.current, new Vector3(target.position.x, position.current.y, target.position.z));
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
}
