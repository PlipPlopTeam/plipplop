using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class MappingWrapper : Dictionary<INPUT, Func<float>>
{
    PlayerIndex index;

    public MappingWrapper(PlayerIndex index)
    {
        var keyCorrespondances = new Dictionary<INPUT, KeyCode>();
        keyCorrespondances.Add(INPUT.KB_Z, KeyCode.Z);
        keyCorrespondances.Add(INPUT.KB_Q, KeyCode.Q);
        keyCorrespondances.Add(INPUT.KB_S, KeyCode.S);
        keyCorrespondances.Add(INPUT.KB_D, KeyCode.D);
        keyCorrespondances.Add(INPUT.KB_DOWN_ARROW, KeyCode.DownArrow);
        keyCorrespondances.Add(INPUT.KB_LEFT_ARROW, KeyCode.LeftArrow);
        keyCorrespondances.Add(INPUT.KB_RIGHT_ARROW, KeyCode.RightArrow);
        keyCorrespondances.Add(INPUT.KB_UP_ARROW, KeyCode.UpArrow);
        keyCorrespondances.Add(INPUT.KB_SPACE, KeyCode.Space);
        keyCorrespondances.Add(INPUT.KB_SHIFT, KeyCode.LeftShift);


        Add(INPUT.NONE, () => { return 0f; });

        /// Numeric buttons
        Add(INPUT.GP_A, () => { return GamePad.GetState(index).Buttons.A == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_B, () => { return GamePad.GetState(index).Buttons.B == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_X, () => { return GamePad.GetState(index).Buttons.X == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_Y, () => { return GamePad.GetState(index).Buttons.Y == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_SELECT, () => { return GamePad.GetState(index).Buttons.Back == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_START, () => { return GamePad.GetState(index).Buttons.Start == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_LB, () => { return GamePad.GetState(index).Buttons.LeftShoulder == ButtonState.Pressed ? 1f : 0f; });
        Add(INPUT.GP_RB, () => { return GamePad.GetState(index).Buttons.RightShoulder == ButtonState.Pressed ? 1f : 0f; });

        /// Analogic buttons
        Add(INPUT.GP_LEFT_X, () => { return GamePad.GetState(index).ThumbSticks.Left.X; });
        Add(INPUT.GP_LEFT_Y, () => { return GamePad.GetState(index).ThumbSticks.Left.Y; });
        Add(INPUT.GP_RIGHT_X, () => { return GamePad.GetState(index).ThumbSticks.Left.X; });
        Add(INPUT.GP_RIGHT_Y, () => { return GamePad.GetState(index).ThumbSticks.Left.Y; });
        Add(INPUT.GP_LT, () => { return GamePad.GetState(index).Triggers.Left; });
        Add(INPUT.GP_RT, () => { return GamePad.GetState(index).Triggers.Right; });

        // Keyboard
        foreach(var c in keyCorrespondances) {
            Add(c.Key, () => { return Convert.ToSingle(Input.GetKey(c.Value)); });
        }
    }
}
