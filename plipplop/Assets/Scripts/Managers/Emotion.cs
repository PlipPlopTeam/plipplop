using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Emotion
{
    public enum EVerb { SEARCH, LOVE, HATE, ANNOYANCE, SURPRISE}
    public enum EBubble { THINK, SAY}

    public EBubble bubbleType;
    public EmotionVerb verb;
    public List<EmotionSubject> subjects;

    public Emotion(EmotionVerb verb, params EmotionSubject[] subjects) : this(EBubble.SAY, verb, subjects) { }

    public Emotion(EBubble bubbleType, EmotionVerb verb, params EmotionSubject[] subjects)
    {
        this.bubbleType = bubbleType;
        this.verb = verb;
        this.subjects = new List<EmotionSubject>();
        if (subjects != null && subjects.Length > 0) {
            this.subjects.AddRange(subjects);
        }
    }

    public override string ToString()
    {
        return "emotion(verb:" + verb.name + "; subjects:" + string.Join(", ", subjects.Select(o=>o.name)) + ")";
    }
}
