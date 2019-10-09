using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mapping", order = 1)]
public class Mapping : ScriptableObject
{
    public PlayerIndex index;
    public List<MappedAction> map = new List<MappedAction>();

    MappingWrapper wrapper;
    Dictionary<ACTION, List<InputWrapper>> inputs = new Dictionary<ACTION, List<InputWrapper>>();

    public Mapping()
    {
        wrapper = new MappingWrapper(index);
        foreach(var action in map) {
            if (!inputs.ContainsKey(action.action)) inputs[action.action] = new List<InputWrapper>();
            inputs[action.action].Add(
                new InputWrapper(
                    action.input,
                    action.isInverted
                ) {
                    factor = action.factor
                }
            );
        }
        map.Clear();
    }

    public float this[ACTION a] {
        get {
            if (!inputs.ContainsKey(a)) throw new System.Exception("Unknown input " + a);
            foreach (var input in inputs[a]) {
                var value = wrapper[input.input]()*(input.isInverted ? -1f : 1f) * input.factor;
                if (value != 0f) {
                    return value;
                }
            }
            return 0f;
        }
    }

    [System.Serializable]
    public class MappedAction{
          public ACTION action;
          public INPUT input;
          public bool isInverted;
          public uint factor=1;
    }
}
