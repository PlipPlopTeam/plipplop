using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool Check(StateManager state);
    }
}
