using UnityEngine;

public class BirdZone : MonoBehaviour
{
	[Header("Settings")]
	public int priority = 0;
	public bool activated = true;
	internal bool full = false;
	private void Awake()
	{
		Game.i.aiZone.Register(this);
	}

	public virtual void Enter()
	{

	}

	public virtual void Exit()
	{

	}

	public virtual bool Available()
    {
		return activated && !full;
	}

	public virtual Bird.Spot GetSpot()
	{
		return null;
	}
}
