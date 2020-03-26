#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(menuName = "ScriptableObjects/AnimationEdit")]
public class AnimationEdit : ScriptableObject
{
    public List<Object> anims;

    [ContextMenu("Clean Interpolation")]
    public void MakeKeyConstant()
    {
        for( int i = 0; i < anims.Count; ++i ) {
            AnimationClip clip = anims[ i ] as AnimationClip;
            EditorCurveBinding[] floatBindings = AnimationUtility.GetCurveBindings( clip );

            for( int j = 0; j < floatBindings.Length; ++j ) {
                EditorCurveBinding binding = floatBindings[ j ];
                AnimationCurve curve = AnimationUtility.GetEditorCurve( clip, binding );

                for( int k = 0; k < curve.keys.Length; ++k ) {
                    AnimationUtility.SetKeyLeftTangentMode( curve, k, AnimationUtility.TangentMode.Constant );
                    AnimationUtility.SetKeyRightTangentMode( curve, k, AnimationUtility.TangentMode.Constant );
                }

                Undo.RecordObject( clip, "Set Constant Tangents" );
                clip.SetCurve( binding.path, typeof( Transform ), binding.propertyName, curve );
            }
        }
    }
}

[CustomEditor(typeof(AnimationEdit)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class AnimationEditEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Make Key Constant"))
        {
            AnimationEdit ae = (AnimationEdit) target;
            ae.MakeKeyConstant();
        }
    }
}


#endif