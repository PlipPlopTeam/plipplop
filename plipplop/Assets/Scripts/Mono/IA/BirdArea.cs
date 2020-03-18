﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BirdArea : MonoBehaviour
{
	public enum Shape { CIRCLE, RECTANGLE }
	[Header("Settings")]
	public Shape shape;
	public float range;
	public Vector2 size;

	public class Spot
	{
		public Transform surface;
		public Vector3 position;

		public Spot(Transform t, Vector3 pos)
		{
			surface = t;
			position = pos;
		}
	}

	private void Awake()
    {
        Game.i.aiZone.Register(this);
    }

	public Spot GetSpot()
	{
		Vector3 position = Vector3.zero;

		if(shape == Shape.CIRCLE)
		{
			position = transform.position + Geometry.GetRandomPointInRange(range);
		}
		else if (shape == Shape.RECTANGLE)
		{
			Vector3 offset = new Vector3(Random.Range(-size.x/2, size.x/2), 0f, Random.Range(-size.y/2, size.y/2));
			position = transform.position + offset;
		}

		RaycastHit[] hits = Physics.RaycastAll(position, Vector3.down);
		foreach (RaycastHit h in hits)
		{
			if (!h.collider.isTrigger) return new Spot(h.collider.transform, h.point);
		}
		return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var style = new GUIStyle();
		style.imagePosition = ImagePosition.ImageAbove;
		style.alignment = TextAnchor.MiddleCenter;
		var tex = Resources.Load<Texture2D>("Editor/Sprites/SPR_flap");
		var content = new GUIContent();
		content.image = tex;
		Handles.Label(transform.position, content, style);
	}

	private void OnDrawGizmosSelected()
	{
		if (shape == BirdArea.Shape.CIRCLE)
		{
			Handles.DrawWireDisc(transform.position, Vector3.up, range);
		}
		else if (shape == BirdArea.Shape.RECTANGLE)
		{
			Handles.DrawWireCube(transform.position, new Vector3(size.x, 0f, size.y));
		}
	}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AIPath)), CanEditMultipleObjects]
[ExecuteInEditMode]
public class AIPathEditor : Editor
{
	public override void OnInspectorGUI()
	{

	}
}
#endif