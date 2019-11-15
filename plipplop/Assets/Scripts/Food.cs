using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    [Header("Food")]
    public FoodData data;
    public bool consumed = false;

    public System.Action onConsumeEnd;
    private bool beingConsumed = false;
    private float timer;

    public override void Start()
    {
        base.Start();

        if(data != null) Create(data);
        if(consumed) Consumed();
    }

    public void Create(FoodData foodData)
    {
        data = foodData;
        if(visual == null) Visual(foodData.visual);
    }

    public override void Destroy()
    {
        Drop();
        base.Destroy();
    }

    public void Consume()
    {
        if(!consumed)
        {
            beingConsumed = true;
            timer = data.timeToConsume;
        }   
    }

    public void Consumed()
    {
        consumed = true;
        if(data.destroyAfterConsumed) Destroy();
    }

    public void Update()
    {
        if(beingConsumed)
        {
            if(timer > 0) timer -= Time.deltaTime;
            else
            {
                if(onConsumeEnd != null)
                {
                    onConsumeEnd.Invoke();
                    onConsumeEnd = null;
                }
                Consumed();
            }
        }
    }
}
