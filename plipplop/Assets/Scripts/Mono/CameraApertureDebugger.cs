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
        var distance = aperture.distance;

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
            Handles.DrawWireDisc(position.current, Vector3.up, settings.range.x);
            Handles.DrawWireDisc(position.current, Vector3.up, settings.range.y);
            Gizmos.DrawLine(cam.transform.position, position.current);
            Gizmos.DrawLine(position.current, target.position);
            Handles.Label(position.current + Vector3.right * settings.range.x, "Min " + settings.range.x.ToString(), style);
            Handles.Label(position.current + Vector3.right * settings.range.y, "Max " + settings.range.y.ToString(), style);
            Handles.Label((cam.transform.position + target.position) / 2, "Dist " + distance.current.ToString(), style);

            Gizmos.DrawLine(cam.transform.position, position.target);
        }

        //Gizmos.DrawLine(transform.position,
        //    transform.position + new Vector3( transform.forward.x, 0f, transform.forward.z)
        //);
    }
    #endregion
}
