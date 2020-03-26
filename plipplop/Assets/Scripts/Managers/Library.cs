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

    [Header("Strings")]
    public string gameName = "Plip plop";

    [Header("Scriptables")]
    public AperturePreset defaultAperture;
    public LocomotionPreset defaultLocomotion;

    [Header("Prefabs")]
    public GameObject legsPrefab;
    public GameObject controllerSensor;
    public GameObject baseControllerPrefab;
	public GameObject teleporterVolumePrefab;
	public GameObject torchPrefab;
    public GameObject canvas;

	[Header("Cloth")]
    public List<ClothData> headClothes;
    public List<ClothData> hairs;
    public List<ClothData> torsoClothes;
    public List<ClothData> legsClothes;
	/*
	public Color[] headColors;
	public Color[] torsoColors;
	public Color[] legsColors;
	*/

	public List<ClothData> GetOutfit()
	{
		List<ClothData> outfit = new List<ClothData>();
		if (Random.Range(0f, 1f) > 0.99f) return outfit;

			if (Random.Range(0f, 1f) > 0.5f) outfit.Add(headClothes[Random.Range(0, headClothes.Count)]);
		if (Random.Range(0f, 1f) > 0.25f) outfit.Add(torsoClothes[Random.Range(0, torsoClothes.Count)]);
		if (Random.Range(0f, 1f) > 0.15f) outfit.Add(hairs[Random.Range(0, hairs.Count)]);
		outfit.Add(legsClothes[Random.Range(0, legsClothes.Count)]);
		return outfit;
	}
	/*
	public Dictionary<Cloth.ESlot, Color> GetRandomOutfitColor()
	{
		Dictionary<Cloth.ESlot, Color> dic = new Dictionary<Cloth.ESlot, Color>();
		dic.Add(Cloth.ESlot.HEAD, headColors.Length > 0 ? headColors[Random.Range(0, headColors.Length)] : Color.white);
		dic.Add(Cloth.ESlot.TORSO, torsoColors.Length > 0 ? torsoColors[Random.Range(0, torsoColors.Length)] : Color.white);
		dic.Add(Cloth.ESlot.LEGS, legsColors.Length > 0 ? legsColors[Random.Range(0, legsColors.Length)] : Color.white);
		return dic;
	}
	*/

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


    // Hidden
    public DialogLibrary dialogs;
}
