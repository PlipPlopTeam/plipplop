using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Emotion")]
public class Emotion : ScriptableObject
{
    public string label;
    public Texture[] frames;
    public float speed;
}
