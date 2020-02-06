using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameFX.SFXParameter))]
public class NamedResourceDrawerWithLabels : NamedResourceDrawer
{
    public NamedResourceDrawerWithLabels()
    {
        displayLabels = true;
    }
}
