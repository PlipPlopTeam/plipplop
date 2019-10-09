using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller), true)]
[CanEditMultipleObjects]
public class ControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("\nPOSSESS\n", EditorStyles.miniButton)) {
            Game.i.player.Possess(((Controller)this.target));
        }
    }
}