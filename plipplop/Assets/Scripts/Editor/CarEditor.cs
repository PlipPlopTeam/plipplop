using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Car), true)]
[CanEditMultipleObjects]
public class CarEditor : ControllerEditor
{
    public override void OnInspectorGUI()
    {
        DrawControllerEditor();

        List<System.Action> buttons = new List<System.Action>();
        var w = Screen.width - wMargin;
        var colWidth = w / columns;
        buttons.Add(AccelerationSlider(colWidth));
        buttons.Add(MaxSpeedSlider(colWidth));
        buttons.Add(SteeringSpeedSlider(colWidth));
        buttons.Add(MaxSteeringSlider(colWidth));
        buttons.Add(SteeringForceSlider(colWidth));
        buttons.Add(AutoBrakeSpeedSlider(colWidth));
        buttons.Add(AntiSpinSpeedSlider(colWidth));
        buttons.Add(AdherenceSpeedSlider(colWidth));

        DrawCustomEditor(buttons, false, "Car properties - Speed & control");

        buttons.Clear();
        buttons.Add(UseAntiFlipButton(colWidth));
        buttons.Add(AntiFlipForceSlider(colWidth));
        buttons.Add(AntiFlipMultiplierSlider(colWidth));
        buttons.Add(AirStabilizationSpeedSlider(colWidth));
        buttons.Add(AirStabilizationSpeedMultiplier(colWidth));
        buttons.Add(FreeFlipButton(colWidth));
        DrawCustomEditor(buttons, false, "Car properties - Flip & tilt");

        DrawDefaultEditor();
        DrawAddLocomotionButton();
    }

    System.Action AccelerationSlider(float width) { return GenericSlider(width, "acceleration", 100f, 5000f, "Acceleration: {0}", "Car_Acceleration"); }
    System.Action MaxSpeedSlider(float width) { return GenericSlider(width, "maxSpeed", 1f, 50f, "Speed: {0}", "Car_Speed"); }
    System.Action SteeringSpeedSlider(float width) { return GenericSlider(width, "steeringSpeed", 1f, 10f, "Steering speed: {0}", "Car_SteeringSpeed"); }
    System.Action MaxSteeringSlider(float width) { return GenericSlider(width, "maxSteering", 0f, 100f, "Max steering: {0}%", "Car_SteeringAmplitude"); }
    System.Action SteeringForceSlider(float width) { return GenericSlider(width, "steeringForce", 50F, 5000f, "Steering force: {0}", "Car_SteeringForce"); }
    System.Action AutoBrakeSpeedSlider(float width) { return GenericSlider(width, "autoBrakeSpeed", 0F, 5F, "Autobrake speed: {0}", "Car_AutoBrakeSpeed"); }
    System.Action AntiSpinSpeedSlider(float width) { return GenericSlider(width, "antiSpinSpeed", 0F, 8F, "Antispin correction: {0}", "Car_AntiSpinSpeed"); }
    System.Action UseAntiFlipButton(float width) { return GenericToggleButton(width, "antiFlip", "Flipping is countered", "Flipping is free", "Car_UseAntiFlip", "Car_UseAntiFlip"); }
    System.Action AntiFlipForceSlider(float width) { return GenericSlider(width, "antiFlipForce", 0F, 3F, "Antiflip force: {0}", "Car_AntiFlipForce"); }
    System.Action AntiFlipMultiplierSlider(float width) { return GenericSlider(width, "antiFlipMultiplier", 0F, 5F, "Antiflip multiplier: {0}", "Car_AntiFlipMultiplier"); }
    System.Action AirStabilizationSpeedSlider(float width) { return GenericSlider(width, "airStabilizationSpeed", 0F, 10F, "Air stabilization speed: {0}", "Car_AirStabilizationSpeed"); }
    System.Action AirStabilizationSpeedMultiplier(float width) { return GenericSlider(width, "airStabilizationMultiplier", 0F, 5F, "Air stabilization mul.: {0}", "Car_AirStabilizationMultiplier"); }
    System.Action FreeFlipButton(float width) { return GenericToggleButton(width, "canTiltWhenFree", "Tilt resets when unposs.", "No tilt reset", "Car_CanTiltWhenFree", "Car_CanTiltWhenFree"); }
    System.Action AdherenceSpeedSlider(float width) { return GenericSlider(width, "highSpeedAdherenceBonusFactor", 0F, 10F, "Speed increases adherence: {0}", "Car_HighSpeedAdherenceBonusFactor"); }

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

            GUILayout.Label(new GUIContent(buttonSpace + "Rigidbody", icon), noBoldTitle, GUILayout.Height(fieldsHeight * 0.66f), GUILayout.ExpandWidth(true));
            serializedObject.FindProperty("customExternalRigidbody").objectReferenceValue = EditorGUILayout.ObjectField(((Controller)target).customExternalRigidbody, typeof(Rigidbody), allowSceneObjects: true);
            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        };
    }
}