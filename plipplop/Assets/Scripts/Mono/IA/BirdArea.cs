using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BirdArea : BirdZone
{
	public enum Shape { CIRCLE, RECTANGLE }
	[Header("Area")]
	public Shape shape;
	public float radius;
	public Vector2 size;
	[Header("Raycast")]
	public float range = 1f;


	public override bool Available()
	{
		return true;
	}

	public override Bird.Spot GetSpot()
	{
		Vector3 position = Vector3.zero;

		if(shape == Shape.CIRCLE)
		{
			position = transform.position + Geometry.GetRandomPointInRange(radius);
		}
		else if (shape == Shape.RECTANGLE)
		{
			Vector3 offset = new Vector3(Random.Range(-size.x/2, size.x/2), 0f, Random.Range(-size.y/2, size.y/2));
			position = transform.position + offset;
		}

		RaycastHit[] hits = Physics.RaycastAll(position, Vector3.down, range);
		for (int i = hits.Length - 1; i > 0; i--)
		{
			if (!hits[i].collider.isTrigger)
				return new Bird.Spot(hits[i].collider.transform, hits[i].point);
		}
		return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		var style = new GUIStyle();
		style.imagePosition = ImagePosition.ImageAbove;
		style.alignment = TextAnchor.UpperCenter;
		var tex = Resources.Load<Texture2D>("Editor/Sprites/SPR_flap");
		var content = new GUIContent();
		content.image = tex;
		Handles.Label(transform.position, content, style);
	}

	private void OnDrawGizmosSelected()
	{
		if (shape == BirdArea.Shape.CIRCLE)
		{
			Handles.DrawWireDisc(transform.position, Vector3.up, radius);
		}
		else if (shape == BirdArea.Shape.RECTANGLE)
		{
			Handles.DrawWireCube(transform.position, new Vector3(size.x, 0f, size.y));
		}

		Handles.DrawLine(transform.position, transform.position + Vector3.down * range);
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