using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NamedPrefabResources : List<NamedPrefabResource>, ISerializationCallbackReceiver
{
    public SerializableNPRs serializableEffects = new SerializableNPRs();

    [Serializable]
    public struct SerializableNPRs
    {
        public NamedPrefabResource[] list;
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

