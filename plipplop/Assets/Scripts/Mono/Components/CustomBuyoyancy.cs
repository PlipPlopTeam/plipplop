using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomBuyoyancy : MonoBehaviour
{
    [Range(0f, 50f)] public float buyoyancy = 1f;
}
