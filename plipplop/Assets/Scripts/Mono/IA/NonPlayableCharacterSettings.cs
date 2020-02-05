using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NonPlayableCharacterSettings")]
public class NonPlayableCharacterSettings : ScriptableObject
{
	[Header("Identity")]
	public string firstname = "Noël";
	public string lastname = "Flantier";

	[Header("Stats")]
	[Range(1f, 2.5f)] public float height = 1.75f;
	[Range(10f, 150f)] public float weight = 65f;
	[Range(0f, 100f)] public float strength = 1f;
	[Range(0f, 100f)] public float age = 25f;

	[Header("Starters")]
	[Range(0f, 100f)] public float initialBoredom = 50f;
	[Range(0f, 100f)] public float initialTiredness = 50f;
	[Range(0f, 100f)] public float initialHunger = 50f;

	[Header("Equipments")]
	public bool autoOutfit = false;
	public bool randomColors = false;
	public ClothData[] clothes;
}
