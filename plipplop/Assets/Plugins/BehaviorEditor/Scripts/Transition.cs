using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP
{
    [System.Serializable]
    public class Transition
    {
        public int id;
        public List<Condition> conditions = new List<Condition>();
        public State targetState;
        public bool disable;
    }
}
