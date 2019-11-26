using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    public abstract class AIAction : ScriptableObject
    {
        public abstract void Execute(StateManager states = null);
    }
}
