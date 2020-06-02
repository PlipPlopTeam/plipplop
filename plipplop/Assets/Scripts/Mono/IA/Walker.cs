using UnityEngine;
using System.Collections.Generic;

public class Walker : MonoBehaviour
{
	[System.Serializable]
	public class SpeedModifiers
	{
		public int id = 0;
		public float multiplier = 1f;

		public SpeedModifiers(int id, float multiplier)
		{
			this.id = id;
			this.multiplier = multiplier;
		}
	}

	public List<SpeedModifiers> modifiers = new List<SpeedModifiers>();

	public int AddModifier(float value)
	{
		modifiers.Add(new SpeedModifiers(modifiers.Count, value));
		ApplyModifier(modifiers[modifiers.Count - 1].multiplier);
		return modifiers[modifiers.Count - 1].id;
	}

	public virtual void ApplyModifier(float value) {}

	public void RemoveModifier(int index)
	{
		int pos = GetModifierPosition(index);
		if(pos != -1)
		{
			ApplyModifier(1f/modifiers[pos].multiplier);
			modifiers.RemoveAt(pos);
		}
	}

	public int GetModifierPosition(int index)
	{
		for(int i = 0; i < modifiers.Count; i++)
		{
			if (index == modifiers[i].id) return i;
		}
		return -1;
	}

	internal Floor floor = null;
	
	/*
	public virtual void FixedUpdate()
	{

		Floor f = FindFloor();
		if (f != null) Enter(f);
		else if(floor != null) Leave();
	}
	*/

	// GET ALL RAYCAST HITS BELOW FEETS
	private RaycastHit[] RaycastAllToGround()
	{
		return Physics.RaycastAll(transform.position, -Vector3.up, 1f);
	}

	// FIND THE FIRST FLOOR BELOW FEETS
	private Floor FindFloor()
	{
		RaycastHit[] hits = RaycastAllToGround();
		foreach(RaycastHit h in hits)
		{
			Floor f = h.collider.gameObject.GetComponent<Floor>();
			if (f != null) return f;
		}
		return null;
	}


	int currentFloorSpeedModifier = -1;
	// ENTER INTO A NEW FLOOR
	private void Enter(Floor f)
	{
		if (floor != null) Leave();
		floor = f;
		currentFloorSpeedModifier = AddModifier(1f - floor.adherence);
	}

	// LEAVE THE CURRENT FLOOR
	private void Leave()
	{
		RemoveModifier(currentFloorSpeedModifier);
		floor = null;
	}
}
