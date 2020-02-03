using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Clothes")]
public class ClothesData : ScriptableObject
{
    public Clothes.ESlot slot;
    public GameObject prefab;
    public Material material;
	public Color color;
}
