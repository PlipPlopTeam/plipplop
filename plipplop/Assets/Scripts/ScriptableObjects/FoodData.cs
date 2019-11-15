using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Food")]
public class FoodData : ScriptableObject
{
    [Header("Assets")]
    public string noon = "Food";
    public GameObject visual;

    [Header("Settings")]
    [Range(0f, 100f)] public float calory = 10f;
    public float timeToConsume = 1f;
    public bool destroyAfterConsumed;
    public float weight = 1f; 
}
