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
    Dictionary<ACTION, List<InputWrapper>> registeredInputs = new Dictionary<ACTION, List<InputWrapper>>();
    Dictionary<ACTION, Input> inputValues = new Dictionary<ACTION, Input>();

    [System.Serializable]
    public class MappedAction
    {
        public ACTION action;
        public INPUT input;
        public bool isInverted;
        public uint factor = 1;
    }

    class Input
    {
        public bool isHeld = false;
        public bool isReleased = false;
        public bool isPressed = false;
    }

    public Mapping()
    {
        wrapper = new MappingWrapper(index);

        // Deserialize from the list
        foreach(var action in map) {
            if (!registeredInputs.ContainsKey(action.action)) registeredInputs[action.action] = new List<InputWrapper>();
            registeredInputs[action.action].Add(
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

    float this[ACTION a] {
        get {
            if (!registeredInputs.ContainsKey(a)) throw new System.Exception("Unknown input " + a);
            foreach (var input in registeredInputs[a]) {
                var value = wrapper[input.input]()*(input.isInverted ? -1f : 1f) * input.factor;
                if (value != 0f) {
                    return value;
                }
            }
            return 0f;
        }
    }

    public Mapping Read()
    {
        foreach(var a in registeredInputs.Keys) {
            if (!inputValues.ContainsKey(a)) inputValues[a] = new Input();

            var input = inputValues[a];
            
            // Released is the same as "was held and is now at 0"
            input.isReleased = input.isHeld && this[a] == 0f;

            // Pressed is "was not held and is now different from 0"
            input.isPressed = !input.isHeld && this[a] != 0f;

            // Held is "is now different from 0"
            input.isHeld = this[a] != 0f;
        }

        return this;
    }

    public bool IsHeld(ACTION a)
    {
        return inputValues[a].isHeld;
    }

    public bool IsPressed(ACTION a)
    {
        return inputValues[a].isPressed;
    }

    public bool IsReleased(ACTION a)
    {
        return inputValues[a].isReleased;
    }

    public float Axis(ACTION a)
    {
        return this[a];
    }
}
