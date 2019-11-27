using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper
{
    public readonly EInput input;
    public readonly bool isInverted = false;
    public uint factor;

    public InputWrapper(EInput input, bool isInverted = false)
    {
        this.input = input;
        this.isInverted = isInverted;
    }
}
