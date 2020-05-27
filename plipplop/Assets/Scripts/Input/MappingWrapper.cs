using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class MappingWrapper : Dictionary<EInput, Func<float>>
{
    PlayerIndex index;

    public MappingWrapper(PlayerIndex index)
    {
        var keyCorrespondances = new Dictionary<EInput, KeyCode>();
        keyCorrespondances.Add(EInput.KB_Z, KeyCode.Z);
        keyCorrespondances.Add(EInput.KB_Q, KeyCode.Q);
        keyCorrespondances.Add(EInput.KB_S, KeyCode.S);
        keyCorrespondances.Add(EInput.KB_D, KeyCode.D);
        keyCorrespondances.Add(EInput.KB_DOWN_ARROW, KeyCode.DownArrow);
        keyCorrespondances.Add(EInput.KB_LEFT_ARROW, KeyCode.LeftArrow);
        keyCorrespondances.Add(EInput.KB_RIGHT_ARROW, KeyCode.RightArrow);
        keyCorrespondances.Add(EInput.KB_UP_ARROW, KeyCode.UpArrow);
        keyCorrespondances.Add(EInput.KB_SPACE, KeyCode.Space);
        keyCorrespondances.Add(EInput.KB_SHIFT, KeyCode.LeftShift);
        keyCorrespondances.Add(EInput.KB_A, KeyCode.A);
        keyCorrespondances.Add(EInput.KB_ALT, KeyCode.LeftAlt);
        keyCorrespondances.Add(EInput.KB_E, KeyCode.E);
        keyCorrespondances.Add(EInput.KB_F, KeyCode.F);
        keyCorrespondances.Add(EInput.KB_X, KeyCode.X);
        keyCorrespondances.Add(EInput.KB_CTRL, KeyCode.LeftControl);


        Add(EInput.NONE, () => { return 0f; });

        /// Numeric buttons
        Add(EInput.GP_A, () => { return GamePad.GetState(index).Buttons.A == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_B, () => { return GamePad.GetState(index).Buttons.B == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_X, () => { return GamePad.GetState(index).Buttons.X == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_Y, () => { return GamePad.GetState(index).Buttons.Y == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_SELECT, () => { return GamePad.GetState(index).Buttons.Back == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_START, () => { return GamePad.GetState(index).Buttons.Start == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_LB, () => { return GamePad.GetState(index).Buttons.LeftShoulder == ButtonState.Pressed ? 1f : 0f; });
        Add(EInput.GP_RB, () => { return GamePad.GetState(index).Buttons.RightShoulder == ButtonState.Pressed ? 1f : 0f; });

        /// Analogic buttons
        Add(EInput.GP_LEFT_X, () => { return GamePad.GetState(index).ThumbSticks.Left.X; });
        Add(EInput.GP_LEFT_Y, () => { return GamePad.GetState(index).ThumbSticks.Left.Y; });
        Add(EInput.GP_RIGHT_X, () => { return GamePad.GetState(index).ThumbSticks.Right.X; });
        Add(EInput.GP_RIGHT_Y, () => { return GamePad.GetState(index).ThumbSticks.Right.Y; });
        Add(EInput.GP_LT, () => { return GamePad.GetState(index).Triggers.Left; });
        Add(EInput.GP_RT, () => { return GamePad.GetState(index).Triggers.Right; });

        // Mouse
        Add(EInput.MS_X, () => { return Input.GetAxis("Mouse X"); });
        Add(EInput.MS_Y, () => { return Input.GetAxis("Mouse Y"); });
        Add(EInput.MS_LMB, () => { return Input.GetMouseButton(0) ? 1f : 0f; });
        Add(EInput.MS_RMB, () => { return Input.GetMouseButton(1) ? 1f : 0f; });

        // Keyboard
        foreach (var c in keyCorrespondances) {
            Add(c.Key, () => { return Convert.ToSingle(Input.GetKey(c.Value)); });
        }
    }
}
