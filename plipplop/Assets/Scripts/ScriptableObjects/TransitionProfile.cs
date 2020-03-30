using UnityEngine;

[CreateAssetMenu(fileName = "TransitionerProfile", menuName = "ScriptableObjects/TransitionerProfile")]
public class TransitionProfile : ScriptableObject
{
	[Header("Speeds")]
	public AnimationCurve closeCurve;
	public AnimationCurve openCurve;
	public float maxCloseDuration;
	public float maxOpenDuration;

	[Header("Materials")]
	public Texture pattern;
	public Color patternColor;
	public Color backgroundColor;
	public Vector2 tilling;
	public Vector2 panningDirection;
	public float panningSpeed;
}
