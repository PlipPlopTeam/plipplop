using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisualEffects : List<VisualEffect>, ISerializationCallbackReceiver
{
    public SerializableVFXs serializableEffects = new SerializableVFXs();

    [Serializable]
    public struct SerializableVFXs
    {
        public VisualEffect[] list; // Do not try to make me generic... Unity doesnt support it
        public int childCount;
    }

    public void OnAfterDeserialize()
    {
        Clear();
        foreach (var sound in serializableEffects.list) {
            Add(sound);
        }
    }

    public void OnBeforeSerialize()
    {
        serializableEffects.list = ToArray();
        serializableEffects.childCount = Count;
    }
}

