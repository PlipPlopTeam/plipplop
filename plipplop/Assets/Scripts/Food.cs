using UnityEngine;

public class Food : Item
{
    [Header("Food")]
    public FoodData data;
    public bool consumed = false;
    public System.Action onConsumeEnd;

	public bool isBeingConsumed { get { return m_BeingConsumed; } }
	bool m_BeingConsumed = false;

    private float timer;

    public float mass;

    public override void Awake()
    {
        base.Awake();
		type = EType.FOOD;
        if(data != null) Create(data);
        if(consumed) Consumed();
    }

    public void Create(FoodData foodData)
    {
        gameObject.name = foodData.name;
        data = foodData;
        if(data.pristine != null) Assemble(data.pristine);
    }

    public void Assemble(GameObject obj)
    {
        Visual(data.pristine);
        if (visuals != null)
        {
            visuals.transform.localEulerAngles = data.rotationOffset;
            visuals.transform.localPosition = data.positionOffset;
        }
    }

    public void Consume(System.Action end)
    {
		onConsumeEnd += end;
		if (!consumed)
        {
			m_BeingConsumed = true;
            timer = data.timeToConsume;
        }   
    }

    public void Consumed()
    {
        End();
        consumed = true;

        if (onConsumeEnd != null)
        {
            onConsumeEnd.Invoke();
            onConsumeEnd = null;
        }
    }

    public void End()
    {
        if (data.consumed != null) Assemble(data.consumed);

        if (data.destroyAfterConsumed) Destroy();
        else type = Item.EType.TRASH;
    }

    public void Update()
    {
        mass = Mass();
        if(m_BeingConsumed)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                
                if(data.scaleDownAsEaten)
                    visuals.transform.localScale = Vector3.one * (timer/data.timeToConsume);
            }
            else Consumed();
        }
    }
}
