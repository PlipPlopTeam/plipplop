using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP
{
    public class StateManager : MonoBehaviour
    {
		public State currentState;
        public virtual void Update()
        {
			if (currentState != null)
            {
                currentState.Tick(this);
            }
        }
    }
}
