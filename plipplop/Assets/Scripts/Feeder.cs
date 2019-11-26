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

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }

    public virtual void Catch(NonPlayableCharacter npc)
    {
        if(stock <= 0) 
            npc.feeder = null;
    }

    public virtual void Serve(NonPlayableCharacter npc)
    {
        if(stock <= 0)
        {
            npc.feeder = null;
            Empty();
            return;
        }
        else stock--;

        Vector3 pos = givePoint == null ? transform.position : givePoint.position;
        Food fo = new GameObject().AddComponent<Food>();
        fo.transform.position = pos;
        fo.Create(food);
        npc.food = fo;
        npc.Carry(fo);

        if(stock <= 0 && destroyOnceEmpty) Destroy(gameObject);
    }

    public virtual void Empty(){}
}
