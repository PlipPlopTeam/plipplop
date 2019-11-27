using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    [System.Serializable]
    public class AIStateTransition : INodeable
    {
        public int id;
        public List<Condition> conditions = new List<Condition>();
        public INodeable outputIfTrue;
        public INodeable outputIfFalse;
        public bool disable;
    }
}
