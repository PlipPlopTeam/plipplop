using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller), true)]
[CanEditMultipleObjects]
public class ControllerEditor : BaseEditor
{
    internal bool unlockedProperties = false;

    public override void OnInspectorGUI()
    {
        DrawControllerEditor();
        DrawDefaultEditor();
        DrawAddLocomotionButton();
    }

    virtual internal void DrawAddLocomotionButton()
    {
        if (!((Controller)target).gameObject.GetComponent<Locomotion>() && GUILayout.Button("Add custom locomotion...", EditorStyles.miniButton)) {
            ((Controller)target).gameObject.AddComponent<Locomotion>();
        }
    }

    virtual internal void DrawDefaultEditor()
    {
        GUILayout.Label("Specific properties", title, GUILayout.Height(headerSeparatorHeight), GUILayout.ExpandWidth(true));

        DrawDefaultInspector();
    }

    internal void DrawControllerEditor()
    {
        MakeStyles();
        var w = Screen.width - wMargin;
        var colWidth = w / columns;
        List<System.Action> buttons = new List<System.Action>();
        buttons.Add(BeginCrouchedButton(colWidth));
        buttons.Add(CanRetractLegsButton(colWidth));
        buttons.Add(UseGravityButton(colWidth));
        buttons.Add(GravityMultiplierSlider(colWidth));
        buttons.Add(CustomCameraField(colWidth));
        buttons.Add(CustomRigidbodyField(colWidth));
        buttons.Add(CustomVisualsField(colWidth));

        if (EditorApplication.isPlaying) {
            EjectButton().Invoke();
        }
        else {
            AutoPossessButton("AUTO-POSSESS").Invoke();
        }

        DrawCustomEditor(buttons);
    }

    virtual internal void DrawCustomEditor(List<System.Action> buttons, bool hasFoldout = true, string foldoutName = "Inherited properties")
    {
        if (hasFoldout) unlockedProperties = EditorGUILayout.BeginFoldoutHeaderGroup(unlockedProperties, foldoutName);
        if (!hasFoldout || unlockedProperties) {
            GUILayout.Label(foldoutName, title, GUILayout.Height(fieldsHeight), GUILayout.ExpandWidth(true));

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.MinHeight(fieldsHeight));

            var currentLine = 0;
            for (int i = 0; i < buttons.Count; i++) {
                if (Mathf.Floor(i / (float)columns) != currentLine) {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(hSpacing);
                    GUILayout.BeginHorizontal(GUILayout.MinHeight(fieldsHeight));
                    currentLine++;
                }
                buttons[i].Invoke();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        if (hasFoldout) EditorGUILayout.EndFoldoutHeaderGroup();
    }

    System.Action EjectButton()
    {
        var obj = (Controller)target;
        var isPossessing = Game.i.player.IsPossessing(obj);

        var possessTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Possess");
        var ejectText = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Eject");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));


        return delegate {
            if (!isPossessing) {
                if (GUILayout.Button(new GUIContent("POSSESS", possessTex), centeredNormalControl, options.ToArray()))
                    Game.i.player.Possess(obj);
            }
            else {
                if (GUILayout.Button(new GUIContent("EJECT", ejectText), centeredNormalControl, options.ToArray()))
                    Game.i.player.TeleportBaseControllerAndPossess();
            }
        };
    }

    bool IsAutoPossessing()
    {
        return ((Controller)target).autoPossess;
    }

    void SetAutoPossess(bool value)
    {
        serializedObject.FindProperty("autoPossess").boolValue = value;
    }

    System.Action AutoPossessButton(string txt)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_NoAutoPossess");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_AutoPossess");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));


        return delegate {
            if (IsAutoPossessing()) {
                if (GUILayout.Button(new GUIContent(txt, asPressedTex), centeredPressedControl, options.ToArray()))
                    SetAutoPossess(false);
            }
            else {
                if (GUILayout.Button(new GUIContent(txt, asNormalTex), centeredNormalControl, options.ToArray()))
                    SetAutoPossess(true);
            }
            serializedObject.ApplyModifiedProperties();
        };
    }

    System.Action BeginCrouchedButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Standing");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Crouching");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.MinHeight(fieldsHeight));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).beginCrouched) {
                if (GUILayout.Button(new GUIContent(buttonSpace+"Begins crouched", asPressedTex), pressedControl))
                    serializedObject.FindProperty("beginCrouched").boolValue = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Begins standing", asNormalTex), normalControl))
                    serializedObject.FindProperty("beginCrouched").boolValue = true;
            }
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action CanRetractLegsButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_CannotRetractLegs");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_CanRetractLegs");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).canRetractLegs) {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Can retract legs", asPressedTex), pressedControl))
                    serializedObject.FindProperty("canRetractLegs").boolValue = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Cannot retract legs", asNormalTex), normalControl))
                    serializedObject.FindProperty("canRetractLegs").boolValue = true;
            }
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action UseGravityButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_NoGravity");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Gravity");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(fieldsHeight));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).useGravity) {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Uses gravity", asPressedTex), pressedControl))
                    serializedObject.FindProperty("useGravity").boolValue = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "No gravity", asNormalTex), normalControl))
                    serializedObject.FindProperty("useGravity").boolValue = true;
            }
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action GravityMultiplierSlider(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.Height(fieldsHeight));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        return delegate {

            var g = ((Controller)target).gravityMultiplier;

            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label(string.Format("Gravity: {0}%", Mathf.Round(g)), noBoldTitle, GUILayout.Height(fieldsHeight * 0.66f), GUILayout.ExpandWidth(true));
            serializedObject.FindProperty("gravityMultiplier").floatValue = GUILayout.HorizontalSlider(g, 1f, 200f, GUILayout.Height(fieldsHeight*0.33f));
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action CustomRigidbodyField(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.MinHeight(fieldsHeight));
        options.Add(GUILayout.ExpandHeight(false));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ControllerRigidbody");

        return delegate {
            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label(new GUIContent(buttonSpace + "Rigidbody", icon), noBoldTitle, GUILayout.Height(fieldsHeight*0.66f), GUILayout.ExpandWidth(true));
            serializedObject.FindProperty("customExternalRigidbody").objectReferenceValue = EditorGUILayout.ObjectField(((Controller)target).customExternalRigidbody, typeof(Rigidbody), allowSceneObjects:true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action CustomCameraField(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.MinHeight(fieldsHeight));
        options.Add(GUILayout.ExpandHeight(false));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ControllerAperture");

        return delegate {
            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label(new GUIContent(buttonSpace + "Aperture preset", icon), noBoldTitle, GUILayout.Height(fieldsHeight*0.66f), GUILayout.ExpandWidth(true));
            serializedObject.FindProperty("customCamera").objectReferenceValue = EditorGUILayout.ObjectField(((Controller)target).customCamera, typeof(AperturePreset), allowSceneObjects: true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }

    System.Action CustomVisualsField(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.MinHeight(fieldsHeight));
        options.Add(GUILayout.ExpandHeight(false));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        var icon = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_ControllerVisuals");


        return delegate {
            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label(new GUIContent(buttonSpace+"Visuals", icon), noBoldTitle, GUILayout.Height(fieldsHeight * 0.66f), GUILayout.ExpandWidth(true));
            serializedObject.FindProperty("visuals").objectReferenceValue = EditorGUILayout.ObjectField(((Controller)target).visuals, typeof(Transform), allowSceneObjects: true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }
}