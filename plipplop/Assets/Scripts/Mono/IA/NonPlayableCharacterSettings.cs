using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NonPlayableCharacterSettings")]
public class NonPlayableCharacterSettings : ScriptableObject
{
	[Header("Identity")]
	public string firstname = "Noël";
	public string lastname = "Flantier";

	[Header("Personality")]
	[Range(0f, 1f)] public float courtesy = 0.5f;
	[Range(0f, 1f)] public float courage = 0.5f;
	[Range(0f, 1f)] public float calm = 0.5f;
	[Range(0f, 1f)] public float attention = 0.5f;
	[Range(0f, 1f)] public float sympathy = 0.5f;

	[Header("Stats")]
	[Range(1f, 2.5f)] public float height = 1.75f;
	[Range(0f, 100f)] public float age = 25f;
	[Range(0f, 100f)] public float strength = 1f;

	[Range(0f, 100f)] public float initialBoredom = 50f;
	[Range(0f, 100f)] public float initialTiredness = 50f;
	[Range(0f, 100f)] public float initialHunger = 50f;

	[Header("Equipments")]
	public ClothesData[] clothes;
}
