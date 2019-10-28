using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Disabled because of an issue with rigidbody
//[CustomEditor(typeof(Rigidbody), true)]
[CanEditMultipleObjects]
public class RigidbodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!((Rigidbody)target).gameObject.GetComponent<CustomBuyoyancy>() && GUILayout.Button("Add custom buyoyancy...", EditorStyles.miniButton)) {
            ((Rigidbody)target).gameObject.AddComponent<CustomBuyoyancy>();
        }
    }
}