using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : MonoBehaviour
{
    [Header("Settings")]
    public Transform givePoint;
    public FoodData food;
    public int stock;
    public bool destroyOnceEmpty = false;

    public virtual void Catch(NonPlayableCharacter npc)
    {
        if(stock <= 0) npc.feeder = null;
    }

    public virtual void Serve(NonPlayableCharacter npc)
    {
        Vector3 pos = givePoint == null ? transform.position : givePoint.position;
        Food fo = new GameObject().AddComponent<Food>();
        fo.transform.position = pos;
        fo.Create(food);
        npc.food = fo;
        npc.Carry(fo);
        stock--;
        if(stock <= 0 && destroyOnceEmpty) Destroy(gameObject);
    }
}
