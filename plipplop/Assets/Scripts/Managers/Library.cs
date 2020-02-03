﻿using System.Collections;
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
	public GameObject torchPrefab;

	[Header("Clothes")]
    public List<ClothesData> headClothes;
    public List<ClothesData> torsoClothes;
    public List<ClothesData> legsClothes;

	public ClothesData[] GetOutfit()
	{
		List<ClothesData> outfit = new List<ClothesData>();

		if(Random.Range(0f, 1f) > 0.5f) outfit.Add(headClothes[Random.Range(0, headClothes.Count)]);
		if (Random.Range(0f, 1f) > 0.25f) outfit.Add(torsoClothes[Random.Range(0, torsoClothes.Count)]);
		outfit.Add(legsClothes[Random.Range(0, legsClothes.Count)]);
		return outfit.ToArray();
	}


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

    [Header("GAMEFX")]
    public GameEffects gfxs;

    [Header("AI")]
    public NpcLibrary npcLibrary;

    [Header("Cinematics")]
    public NamedPrefabResources cinematics;
}
