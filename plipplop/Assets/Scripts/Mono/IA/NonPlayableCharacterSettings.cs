using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NonPlayableCharacterSettings")]
public class NonPlayableCharacterSettings : ScriptableObject
{
	[Header("Identity")]
	public string firstname = "Noël";
	public string lastname = "Flantier";

	[Header("Stats")]
	public bool randomStats = false;
	[Range(1f, 2.5f)] public float height = 1.75f;
	public Vector2 rangeHeight;
	[Range(10f, 150f)] public float weight = 65f;
	public Vector2 rangeWeight;
	[Range(0f, 100f)] public float age = 25f;
	public Vector2 rangeAge;
	[Range(0f, 100f)] public float strength = 1f;

	/*
	[Header("Starters")]
	[Range(0f, 100f)] public float initialBoredom = 50f;
	[Range(0f, 100f)] public float initialTiredness = 50f;
	[Range(0f, 100f)] public float initialHunger = 50f;
	*/

	[Header("Equipments")]
	public bool autoOutfit = false;
	public bool randomColors = false;
	public ClothData[] clothes;

	[Header("Behavior")]
	public bool followPath;
	public string[] favoriteActivities;
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
