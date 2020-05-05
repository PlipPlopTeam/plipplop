using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Emotions
{
    [Serializable]
    public class BubbleSprite
    {
        public Emotion.EBubble bubbleType;
        public Texture texture;
    }

    public List<SerializableEmotionVerb> emotionVerbs = new List<SerializableEmotionVerb>();
    public List<EmotionSubject> subjects = new List<EmotionSubject>();
    public List<BubbleSprite> bubbleSprites = new List<BubbleSprite>();

    public EmotionVerb GetVerb(Emotion.EVerb verbId) { 
        var v = emotionVerbs.Find(o => o.id == verbId);
        return v!=null ? v.verb : throw new Exception("VERB NOT FOUND: "+verbId+" (Check the library)");
    }

    public EmotionSubject GetSubject(string name)
    {
        var v = subjects.Find(o => o.name == name);
        return v != null ? v : throw new Exception("SUBJECT NOT FOUND: [" + name + "] (Check the library)");
    }

    public Texture GetBubbleSprite(Emotion.EBubble bubbleType)
    {
        var v = bubbleSprites.Find(o => o.bubbleType == bubbleType).texture;
        return v != null ? v : throw new Exception("BUBBLE SPRITE NOT FOUND: [" + bubbleType + "] (Check the library)");
    }
}
