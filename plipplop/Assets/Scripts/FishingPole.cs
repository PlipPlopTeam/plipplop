using UnityEngine;

public class FishingPole : Item
{
	public Transform poleEnd;
	public Transform plug;
	public LineRenderer line;

	bool isBeingUsed;

	public void Use()
	{
		isBeingUsed = true;
		line.enabled = false;
		plug.gameObject.SetActive(true);
	}

	public override void Drop()
	{
		base.Drop();
		isBeingUsed = false;
		line.enabled = false;
		plug.gameObject.SetActive(false);
		plug.SetParent(transform);
	}

	public void Plunge(Vector3 position)
	{
		line.enabled = true;
		plug.parent = null;
		plug.position = position;
	}

	public void Update()
	{
		if(isBeingUsed)
		{
			line.SetPositions(new Vector3[] { poleEnd.position, plug.position });
			transform.up = Vector3.up;
		}
	}
}
