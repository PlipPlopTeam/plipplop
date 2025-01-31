﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using TMPro;

public class AreaDebugText : MonoBehaviour
{
    public enum EFacing { TOP, FRONT}

    public string text;
    public EFacing facing;
    TextMeshPro tmp;

    private void OnValidate()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
        tmp.text = text;

        tmp.transform.localRotation = facing == EFacing.TOP ? Quaternion.Euler(90f, 180f, 0f) : Quaternion.Euler(0f, 180f, 0f);
        tmp.transform.localPosition = (Vector3.one - Vector3.right) * 0.01f;
    }
}
