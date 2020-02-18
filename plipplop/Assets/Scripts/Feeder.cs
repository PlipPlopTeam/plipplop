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
	public float range = 3f;

	public bool IsEmpty()
	{
		return stock > 0;
	}

    private void Start()
    {
        Game.i.aiZone.Register(this);
    }

    public virtual void Catch(NonPlayableCharacter npc)
    {
        if(stock <= 0) 
            npc.feeder = null;
    }

	public virtual bool InRange(NonPlayableCharacter npc)
	{
		return Vector3.Distance(npc.transform.position, transform.position) <= range;
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
