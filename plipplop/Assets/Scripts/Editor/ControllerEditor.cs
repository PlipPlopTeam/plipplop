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

        if (!((Controller)target).gameObject.GetComponent<Locomotion>() && GUILayout.Button("Add custom locomotion", EditorStyles.miniButton)) {
            ((Controller)target).gameObject.AddComponent<Locomotion>();
        }

        if (((Controller)target).autoPossess) {
            foreach(var c in Resources.FindObjectsOfTypeAll<Controller>()) {
                if (c != target) {
                    c.autoPossess = false;
                }
            }
        }

        if (EditorApplication.isPlaying) {

            var obj = (Controller)this.target;
            var isPossessing = Game.i.player.IsPossessing(obj);

            if (isPossessing && GUILayout.Button("\nEJECT\n", EditorStyles.miniButton)) {
                Game.i.player.Eject();
            }
            if (!isPossessing && GUILayout.Button("\nPOSSESS\n", EditorStyles.miniButton)) {
                Game.i.player.Possess(obj);
            }
        }
    }
}