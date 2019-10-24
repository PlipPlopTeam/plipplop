using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Emotion")]
public class Emotion : ScriptableObject
{
    public string name;
    public Texture[] frames;
    public float speed;
}
