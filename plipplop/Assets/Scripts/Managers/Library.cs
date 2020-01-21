using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Library
{
    [System.Serializable]
    public class Resource<T>
    {
        public int id;
        public T resource;
    }

    [System.Serializable]
    public class GenericNamedPrefabResource
    {
        public string name;
        public GameObject prefab;
    }

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
    public GameObject teleporterVolumePrefab;

    [Header("Clothes")]
    public List<ClothesData> headClothes;
    public List<ClothesData> torsoClothes;
    public List<ClothesData> legsClothes;


    [Header("Material")]
    public Material emotionBoardMaterial;
    public Material lineRendererMaterial;
    public Material killZMaterial;
    
    [Header("Meshs")]
    public Mesh primitiveQuadMesh;

    [Header("Sounds")]
    public Sounds sounds;

    [Header("VFX")]
    public VisualEffects vfxs;

    [Header("AI")]
    public NpcLibrary npcLibrary;

    [Header("Cinematics")]
    public VisualEffects cinematics; // Yes this class is called visual effects, yes its normal
}
