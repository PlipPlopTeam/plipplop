using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputWrapper
{
    public readonly INPUT input;
    public readonly bool isInverted = false;
    public uint factor;

    public InputWrapper(INPUT input, bool isInverted = false)
    {
        this.input = input;
        this.isInverted = isInverted;
    }
}
