using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mapping", order = 1)]
public class Mapping : ScriptableObject
{
    [System.Serializable]
    public class MappedAction
    {
        public EAction action;
        public EInput input;
        public bool isInverted;
        public uint factor = 1;
    }

    public List<MappedAction> map;
    public PlayerIndex index;

    MappingWrapper wrapper;
    Dictionary<EAction, List<InputWrapper>> registeredInputs = new Dictionary<EAction, List<InputWrapper>>();
    Dictionary<EAction, Input> inputValues = new Dictionary<EAction, Input>();

    class Input
    {
        public bool isHeld = false;
        public bool isReleased = false;
        public bool isPressed = false;
    }

    private void Awake()
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
    }

    float this[EAction a] {
        get {
            if (!registeredInputs.ContainsKey(a)) throw new System.Exception("Unknown input " + a);
            foreach (var input in registeredInputs[a]) {
                try {
                    var value = wrapper[input.input]() * (input.isInverted ? -1f : 1f) * input.factor;
                    if (value != 0f) {
                        return value;
                    }
                }
                catch (KeyNotFoundException) {
                    throw new System.Exception(string.Format("Unregistered input: {0}. Check the mapping wrapper.", input.input));
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

    public bool IsHeld(EAction a)
    {
        try {
            return inputValues[a].isHeld;
        }
        catch (KeyNotFoundException) { throw new System.Exception("UNKNOWN KEY " + a); }
    }

    public bool IsPressed(EAction a)
    {
        try {
            return inputValues[a].isPressed;
        }
        catch (KeyNotFoundException) { throw new System.Exception("UNKNOWN KEY " + a); }
    }

    public bool IsReleased(EAction a)
    {
        try {
            return inputValues[a].isReleased;
        }
        catch (KeyNotFoundException) { throw new System.Exception("UNKNOWN KEY " + a); }
    }

    public float Axis(EAction a)
    {
        try {
            return this[a];
        }
        catch (KeyNotFoundException) { throw new System.Exception("UNKNOWN AXIS " + a); }
    }
}
