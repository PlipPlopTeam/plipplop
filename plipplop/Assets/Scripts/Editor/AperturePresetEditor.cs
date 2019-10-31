using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AperturePreset), true)]
[CanEditMultipleObjects]
public class AperturePresetEditor : Editor
{
    bool unlockedProperties = false;

    readonly float fieldsHeight = 35f;
    readonly float hSpacing = 10f;
    readonly float headerSeparatorHeight = 30f;
    readonly float wMargin = 60f;
    readonly int columns = 2;

    float overrideColumnWidth = 60f;

    GUIStyle title;
    GUIStyle box;
    GUIStyle boxToggle;
    GUIStyle pressedControl;
    GUIStyle normalControl;
    GUIStyle centeredPressedControl;
    GUIStyle centeredNormalControl;
    GUIStyle toggle;

    string buttonSpace = "   ";

    Dictionary<string, InheritableProperties> inheritableProperties = new Dictionary<string, InheritableProperties>();

    class InheritableProperty
    {
        public bool inheritDefault = true;
        public SerializedProperty property;
        public string name { get { return property.name; } }
    }

    class InheritableProperties : List<InheritableProperty>
    {
        public InheritableProperties() { }
        public InheritableProperties(Dictionary<string, bool> overrides) { this.overrides = overrides; }

        Dictionary<string, bool> overrides = new Dictionary<string, bool>();

        public void Add(SerializedProperty prop)
        {
            if (Find(o=>o.property.name == prop.name) == null) {
                Add(new InheritableProperty() { property = prop, inheritDefault = overrides.ContainsKey(prop.name) && !overrides[prop.name] });
            }
        }

        public void Sanitize(SerializedObject obj)
        {
            foreach(var p in ToArray()) {
                if (
                    p.property == null || 
                    obj.FindProperty(p.property.name) == null
                ) {
                   RemoveAll(o => o.property == p.property);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        MakeStyles();
        LoadProperties();

        bool isDefault = target.name == "DefaultAperture";

        EditorGUI.BeginDisabledGroup(isDefault);
        EditorGUILayout.BeginVertical(title);
        GUILayout.Label("default aperture".ToUpper(), title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fallback"));
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUI.EndDisabledGroup();

        var fb = ((AperturePreset)target).fallback;
        if (!fb && !isDefault) {
            GUILayout.Label("Please pick a fallback for this aperture");
            serializedObject.ApplyModifiedProperties();
            return;
        }

        var overrides = ((AperturePreset)target).overrides;
        foreach (var category in inheritableProperties.Keys) {

            EditorGUILayout.BeginVertical(title);
            GUILayout.Label(category.ToUpper(), title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Override", EditorStyles.boldLabel, GUILayout.Width(overrideColumnWidth));
            EditorGUILayout.LabelField("Property", EditorStyles.boldLabel, GUILayout.ExpandWidth(true), GUILayout.MinWidth(Screen.width * 0.8f));
            EditorGUILayout.EndHorizontal();

            SerializedObject serializedDefault = null;
            if (fb) {
                serializedDefault = new SerializedObject(fb);
            }

            foreach (var ip in inheritableProperties[category]) {
                var defaultProperty = fb == null ? null : serializedDefault.FindProperty(ip.name);
                EditorGUILayout.BeginHorizontal();
                if (isDefault) ip.inheritDefault = false;
                if (ip.inheritDefault && GUILayout.Button("AUTO", normalControl, GUILayout.Width(overrideColumnWidth))) {
                    ip.inheritDefault = false;
                }
                if (!ip.inheritDefault && GUILayout.Button("OVERRIDE", pressedControl, GUILayout.Width(overrideColumnWidth))) {
                    ip.inheritDefault = true;
                }
                overrides[ip.name] = !ip.inheritDefault;

                EditorGUI.BeginDisabledGroup(ip.inheritDefault);
                EditorGUILayout.PropertyField(ip.inheritDefault && fb ? defaultProperty : ip.property, new GUIContent(ip.property.displayName), GUILayout.ExpandWidth(true));
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
        return;




        List<System.Action> fields = new List<System.Action>();
        var w = Screen.width - wMargin;
        var colWidth = w / columns;

        unlockedProperties = EditorGUILayout.BeginFoldoutHeaderGroup(unlockedProperties, "Inherited properties");
        if (unlockedProperties) {
            GUILayout.Label("Inherited properties", title, GUILayout.Height(20f), GUILayout.ExpandWidth(true));

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.MinHeight(fieldsHeight));

            var currentLine = 0;
            for (int i = 0; i < fields.Count; i++) {
                if (Mathf.Floor(i/(float)columns) != currentLine) {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(hSpacing);
                    GUILayout.BeginHorizontal(GUILayout.MinHeight(fieldsHeight));
                    currentLine++;
                }
                fields[i].Invoke();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GUILayout.Label("Specific properties", title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));

        DrawDefaultInspector();


    }

    void LoadProperties()
    {
        string switches = "switches";
        string basicParameters = "basic parameters";

        var overrides = ((AperturePreset)target).overrides;

        if (!inheritableProperties.ContainsKey(switches)) inheritableProperties[switches] = new InheritableProperties(overrides);
        if (!inheritableProperties.ContainsKey(basicParameters)) inheritableProperties[basicParameters] = new InheritableProperties(overrides);

        inheritableProperties[switches].Add(serializedObject.FindProperty("canBeControlled"));
        inheritableProperties[switches].Sanitize(serializedObject);

        inheritableProperties[basicParameters].Add(serializedObject.FindProperty("fieldOfView"));
        inheritableProperties[basicParameters].Add(serializedObject.FindProperty("heightOffset"));
        inheritableProperties[basicParameters].Add(serializedObject.FindProperty("additionalAngle"));
        inheritableProperties[basicParameters].Add(serializedObject.FindProperty("distance"));
        inheritableProperties[basicParameters].Sanitize(serializedObject);

    }

    void MakeStyles()
    {
        title = new GUIStyle(GUI.skin.box);
        title.fontSize = 12;
        title.alignment = TextAnchor.MiddleCenter;
        title.normal.textColor = Color.white;

        box = new GUIStyle(GUI.skin.box);
        box.normal.textColor = Color.white;
        box.alignment = TextAnchor.MiddleCenter;

        boxToggle = new GUIStyle(box);
        boxToggle.active.background = GUI.skin.toggle.active.background;
        boxToggle.normal.background = GUI.skin.toggle.normal.background;

        toggle = new GUIStyle(GUI.skin.toggle);
        toggle.alignment = TextAnchor.MiddleCenter;

        normalControl = new GUIStyle(GUI.skin.button);
        normalControl.fontSize = 8;
        normalControl.normal.textColor = Color.Lerp(Color.white, Color.blue, 0.5f);
        normalControl.alignment = TextAnchor.MiddleCenter;

        pressedControl = new GUIStyle(GUI.skin.button);
        pressedControl.normal = GUI.skin.button.active;
        pressedControl.fontSize = 8;
        pressedControl.normal.textColor = Color.Lerp(Color.white, Color.green, 0.5f);
        pressedControl.alignment = TextAnchor.MiddleCenter;

        centeredNormalControl = new GUIStyle(normalControl);
        centeredNormalControl.fontStyle = FontStyle.Bold;
        centeredNormalControl.alignment = TextAnchor.MiddleCenter;

        centeredPressedControl = new GUIStyle(pressedControl);
        centeredPressedControl.fontStyle = FontStyle.Bold;
        centeredPressedControl.alignment = TextAnchor.MiddleCenter;


    }
}