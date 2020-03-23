using UnityEngine;

public class BirdZone : MonoBehaviour
{
    public int priority = 0;

	private void Awake()
	{
		Game.i.aiZone.Register(this);
	}

	public virtual bool Available()
    {
		return false;
	}

	public virtual Bird.Spot GetSpot()
	{
		return null;
	}
}
