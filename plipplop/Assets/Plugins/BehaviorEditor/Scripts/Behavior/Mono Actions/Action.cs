using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP
{
    public abstract class AIAction : ScriptableObject
    {
        public abstract void Execute(StateManager states = null);
    }
}
