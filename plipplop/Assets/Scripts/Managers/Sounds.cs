using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sounds : List<Sound>, ISerializationCallbackReceiver
{
    public SerializableSounds serializableSounds = new SerializableSounds();

    [Serializable]
    public struct SerializableSounds
    {
        public Sound[] list;
        public int childCount;
    }

    public void OnAfterDeserialize()
    {
        Clear();
        foreach(var sound in serializableSounds.list) {
            Add(sound);
        }
    }

    public void OnBeforeSerialize()
    {
        serializableSounds.list = ToArray();
        serializableSounds.childCount = Count;
    }
}

