using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameEffects : List<GameEffect>, ISerializationCallbackReceiver
{
    public SerializableFXs serializableEffects = new SerializableFXs();

    [Serializable]
    public struct SerializableFXs
    {
        public GameEffect[] list;
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

    public GameFX this[string name]{
        get {
            var gfx = Find(o => o.name == name);
            if (gfx == null) {
                throw new Exception("Attempted to get gfx " + name + " that does NOT exist. Please check the library.");
            }
            return gfx.gfxScriptableObject;
        }
    }
}

