using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/NonPlayableCharacterSettings")]
public class NonPlayableCharacterSettings : ScriptableObject
{
	[Header("Identity")]
	public string firstname = "Noël";
	public string lastname = "Flantier";

	[Header("Stats")]
	[Range(1f, 2.5f)] public float height = 1.75f;
	[Range(10f, 150f)] public float weight = 65f;
	[Range(0f, 100f)] public float age = 25f;
	[Range(0f, 100f)] public float strength = 1f;

	public bool randomStats = false;
	public Vector2 rangeHeight;
	public Vector2 rangeWeight;
	public Vector2 rangeAge;

	[Header("Equipments")]
	public bool autoOutfit = false;
	public bool randomColors = false;
	public List<ClothData> clothes = new List<ClothData>();

	[Header("Behavior")]
	public bool followPath;
	public bool doAllActivities;
	public bool canDoActivitiesTwice = false;
	public List<string>  favoriteActivities = new List<string>();
	/*
#if UNITY_EDITOR
	public GameObject prefab;
#endif
*/
	public float GetWeightRatio()
	{
		return (10f + weight) / (150f + 10f);
	}

	public void Load()
	{
		if(randomStats)
		{
			height = Random.Range(rangeHeight.x, rangeHeight.y);
			weight = Random.Range(rangeWeight.x, rangeWeight.y);
			age = Random.Range(rangeAge.x, rangeAge.y);
		}
	}
}
