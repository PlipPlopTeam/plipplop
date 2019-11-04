using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using TMPro;

public class AreaDebugText : MonoBehaviour
{
    public string text;
    TextMeshPro tmp;

    private void OnValidate()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
        tmp.text = text;
    }
}
