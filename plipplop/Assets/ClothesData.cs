using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Clothes")]
public class ClothesData : ScriptableObject
{
    public string noun;
    public Clothes.Slot slot;
    public GameObject prefab;
}
