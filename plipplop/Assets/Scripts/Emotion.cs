using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Emotion")]
public class Emotion : ScriptableObject
{
    public Texture[] frames;
    public float speed;
}
