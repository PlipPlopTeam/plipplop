using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseEditor : Editor
{

    internal readonly float fieldsHeight = 35f;
    internal readonly float hSpacing = 10f;
    internal readonly float headerSeparatorHeight = 30f;
    internal readonly float wMargin = 60f;
    internal readonly int columns = 2;

    internal string buttonSpace = "   ";

    internal GUIStyle title;
    internal GUIStyle pressedControl;
    internal GUIStyle normalControl;
    internal GUIStyle centeredPressedControl;
    internal GUIStyle centeredNormalControl;


    internal void MakeStyles()
    {
        title = new GUIStyle(GUI.skin.box);
        title.fontSize = 12;
        title.alignment = TextAnchor.MiddleCenter;
        title.normal.textColor = Color.white;

        normalControl = new GUIStyle(GUI.skin.button);
        normalControl.fontSize = 12;
        normalControl.normal.textColor = Color.white;
        normalControl.margin = new RectOffset(14, 14, 14, 14);
        normalControl.alignment = TextAnchor.MiddleLeft;

        pressedControl = new GUIStyle(GUI.skin.button);
        pressedControl.normal = GUI.skin.button.active;
        pressedControl.fontSize = 12;
        pressedControl.margin = new RectOffset(14, 14, 14, 14);
        pressedControl.normal.textColor = Color.Lerp(Color.white, Color.green, 0.5f);
        pressedControl.alignment = TextAnchor.MiddleLeft;

        centeredNormalControl = new GUIStyle(normalControl);
        centeredNormalControl.fontStyle = FontStyle.Bold;
        centeredNormalControl.alignment = TextAnchor.MiddleCenter;

        centeredPressedControl = new GUIStyle(pressedControl);
        centeredPressedControl.fontStyle = FontStyle.Bold;
        centeredPressedControl.alignment = TextAnchor.MiddleCenter;


    }


    internal System.Action GenericSlider(float width, string propertyName, float min, float max, string displayFormat, string spriteName)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var tex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_" + spriteName);

        return delegate {
            try {
                var a = serializedObject.FindProperty(propertyName).floatValue;

                GUILayout.BeginVertical(options.ToArray());

                GUILayout.Label(new GUIContent(buttonSpace + string.Format(displayFormat, Mathf.Round(a)), tex), noBoldTitle, GUILayout.ExpandWidth(true));
                serializedObject.FindProperty(propertyName).floatValue = GUILayout.HorizontalSlider(a, min, max, GUILayout.Height(fieldsHeight * 0.33f));
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndVertical();
            }
            catch (System.NullReferenceException) {
                Debug.LogError("!! INVALID PROPERTY FOR " + target.name + ": " + propertyName);
            }
        };
    }

    internal System.Action GenericToggleButton(float width, string propertyName, string nameActive, string nameInactive, string spriteNameActive, string spriteNameInactive)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_" + spriteNameInactive);
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_" + spriteNameActive);

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (serializedObject.FindProperty(propertyName).boolValue) {
                if (GUILayout.Button(new GUIContent(buttonSpace + nameActive, asPressedTex), pressedControl))
                    serializedObject.FindProperty(propertyName).boolValue = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + nameInactive, asNormalTex), normalControl))
                    serializedObject.FindProperty(propertyName).boolValue = true;
            }
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }
}
