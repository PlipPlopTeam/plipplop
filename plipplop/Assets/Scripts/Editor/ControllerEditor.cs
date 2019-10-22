using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller), true)]
[CanEditMultipleObjects]
public class ControllerEditor : Editor
{
    bool unlockedProperties = false;
    int columns = 2;
    GUIStyle title;

    GUIStyle pressedControl;
    GUIStyle normalControl;
    GUIStyle centeredPressedControl;
    GUIStyle centeredNormalControl;

    string buttonSpace = "   ";

    public override void OnInspectorGUI()
    {
        MakeStyles();

        List<System.Action> buttons = new List<System.Action>();
        var w = Screen.width - 60f;
        var colWidth = w / columns;
        buttons.Add(BeginCrouchedButton(colWidth));
        buttons.Add(CanRetractLegsButton(colWidth));
        buttons.Add(UseGravityButton(colWidth));
        buttons.Add(GravityMultiplierSlider(colWidth));
        buttons.Add(CustomCameraField(colWidth));
        buttons.Add(CustomRigidbodyField(colWidth));


        if (EditorApplication.isPlaying) {
            EjectButton().Invoke();
        }
        else {
            AutoPossessButton("AUTO-POSSESS").Invoke();
        }

        unlockedProperties = EditorGUILayout.BeginFoldoutHeaderGroup(unlockedProperties, "Inherited properties");
        if (unlockedProperties) {
            GUILayout.Label("Inherited properties", title, GUILayout.Height(20f), GUILayout.ExpandWidth(true));

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.Height(35f));

            var currentLine = 0;
            for (int i = 0; i < buttons.Count; i++) {
                if (Mathf.Floor(i/(float)columns) != currentLine) {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.Height(35f));
                    currentLine++;
                }
                buttons[i].Invoke();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GUILayout.Label("Specific properties", title, GUILayout.Height(30f), GUILayout.ExpandWidth(true));


        if (!((Controller)target).gameObject.GetComponent<Locomotion>() && GUILayout.Button("Add custom locomotion...", EditorStyles.miniButton)) {
            ((Controller)target).gameObject.AddComponent<Locomotion>();
        }

        DrawDefaultInspector();
    }

    System.Action EjectButton()
    {
        var obj = (Controller)target;
        var isPossessing = Game.i.player.IsPossessing(obj);

        var possessTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Possess");
        var ejectText = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Eject");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(35f));


        return delegate {
            if (!isPossessing) {
                if (GUILayout.Button(new GUIContent("POSSESS", possessTex), centeredNormalControl, options.ToArray()))
                    Game.i.player.Possess(obj);
            }
            else {
                if (GUILayout.Button(new GUIContent("EJECT", ejectText), centeredNormalControl, options.ToArray()))
                    Game.i.player.PossessBaseController();
            }
        };
    }

    void MakeStyles()
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

    bool IsAutoPossessing()
    {
        return ((Controller)target).autoPossess;
    }

    void SetAutoPossess(bool value)
    {
        ((Controller)target).autoPossess = value;

        foreach (var c in Resources.FindObjectsOfTypeAll<Controller>()) {
            if (c != target) {
                c.autoPossess = false;
            }
        }
    }

    System.Action AutoPossessButton(string txt)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_NoAutoPossess");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_AutoPossess");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(35f));


        return delegate {
            if (IsAutoPossessing()) {
                if (GUILayout.Button(new GUIContent(txt, asPressedTex), centeredPressedControl, options.ToArray()))
                    SetAutoPossess(false);
            }
            else {
                if (GUILayout.Button(new GUIContent(txt, asNormalTex), centeredNormalControl, options.ToArray()))
                    SetAutoPossess(true);
            }
        };
    }

    System.Action BeginCrouchedButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Standing");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Crouching");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(35f));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).beginCrouched) {
                if (GUILayout.Button(new GUIContent(buttonSpace+"Begins crouched", asPressedTex), pressedControl))
                    ((Controller)target).beginCrouched = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Begins standing", asNormalTex), normalControl))
                    ((Controller)target).beginCrouched = true;
            }
            GUILayout.EndVertical();
        };
    }

    System.Action CanRetractLegsButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_CannotRetractLegs");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_CanRetractLegs");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(35f));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).canRetractLegs) {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Can retract legs", asPressedTex), pressedControl))
                    ((Controller)target).canRetractLegs = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Cannot retract legs", asNormalTex), normalControl))
                    ((Controller)target).canRetractLegs = true;
            }
            GUILayout.EndVertical();
        };
    }

    System.Action UseGravityButton(float width)
    {
        // Auto possess
        var asNormalTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_NoGravity");
        var asPressedTex = Resources.Load<Texture2D>("Editor/Sprites/SPR_D_Gravity");

        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Height(35f));
        options.Add(GUILayout.Width(width));

        return delegate {
            GUILayout.BeginVertical(options.ToArray());
            if (((Controller)target).useGravity) {
                if (GUILayout.Button(new GUIContent(buttonSpace + "Uses gravity", asPressedTex), pressedControl))
                    ((Controller)target).useGravity = false;
            }
            else {
                if (GUILayout.Button(new GUIContent(buttonSpace + "No gravity", asNormalTex), normalControl))
                    ((Controller)target).useGravity = true;
            }
            GUILayout.EndVertical();
        };
    }

    System.Action GravityMultiplierSlider(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.Height(35f));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;

        return delegate {

            var g = ((Controller)target).gravityMultiplier;

            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label(string.Format("Gravity: {0}%", Mathf.Round(g)), noBoldTitle, GUILayout.Height(20f), GUILayout.ExpandWidth(true));
            ((Controller)target).gravityMultiplier = GUILayout.HorizontalSlider(g, 1f, 200f, GUILayout.Height(15f));

            GUILayout.EndVertical();
        };
    }

    System.Action CustomRigidbodyField(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.Height(35f));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;


        return delegate {
            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label("Custom rigidbody", noBoldTitle, GUILayout.Height(20f), GUILayout.ExpandWidth(true));
            ((Controller)target).customExternalRigidbody = (Rigidbody)EditorGUILayout.ObjectField(((Controller)target).customExternalRigidbody, typeof(Rigidbody), allowSceneObjects:true);
            GUILayout.EndVertical();
        };
    }

    System.Action CustomCameraField(float width)
    {
        var options = new List<GUILayoutOption>();

        options.Add(GUILayout.Width(width));
        options.Add(GUILayout.Height(35f));

        var noBoldTitle = new GUIStyle(title);
        noBoldTitle.fontStyle = FontStyle.Normal;


        return delegate {
            GUILayout.BeginVertical(options.ToArray());

            GUILayout.Label("Custom aperture preset", noBoldTitle, GUILayout.Height(20f), GUILayout.ExpandWidth(true));
            ((Controller)target).customCamera = (AperturePreset)EditorGUILayout.ObjectField(((Controller)target).customCamera, typeof(AperturePreset), allowSceneObjects: true);
            GUILayout.EndVertical();
        };
    }
}