using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Library
{
    [Header("Strings")]
    public string gameName = "Plip plop";

    [Header("Scriptables")]
    public AperturePreset defaultAperture;
    public LocomotionPreset defaultLocomotion;

    [Header("Prefabs")]
    public GameObject legsPrefab;
    public GameObject facePrefab;
    public GameObject controllerSensor;
    public GameObject baseControllerPrefab;

    [Header("Material")]
    public Material emotionBoardMaterial;
    public Material lineRendererMaterial;
    
    [Header("Meshs")]
    public Mesh primitiveQuadMesh;

}
