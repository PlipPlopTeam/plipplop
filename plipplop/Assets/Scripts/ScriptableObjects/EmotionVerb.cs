using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/Emotion/Verb")]
public class EmotionVerb : ScriptableObject
{
    public Texture[] frames;
    public string[] sounds;
}
