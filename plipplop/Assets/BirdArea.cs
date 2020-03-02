using UnityEngine;
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

	public Vector3 GetLandPosition()
	{
		Vector3 position = transform.position + Geometry.GetRandomPointInRange(range);
		RaycastHit hit;
		if (Physics.Raycast(position, Vector3.down, out hit)) return hit.point;
		else return transform.position;
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