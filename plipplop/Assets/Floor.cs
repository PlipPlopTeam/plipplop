using UnityEngine;

public class Floor : MonoBehaviour
{
	public enum EMaterial { SAND, DIRT, CONCRETE, WOOD };

	[Header("Settings")]
	public EMaterial material;
	[Range(0f, 1f)] public float adherence;
}
