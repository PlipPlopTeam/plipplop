using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PP.Behavior
{
    [CreateAssetMenu(menuName = "Behavior/Graph")]
    public class BehaviorGraph : ScriptableObject
    {
		[SerializeField] public List<Node> nodes = new List<Node>();
		[SerializeField] public int idCount;
        List<int> indexToDelete = new List<int>();

        #region Checks
        public Node GetNodeWithIndex(int index)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id == index)
                    return nodes[i];
            }
            return null;
        }

        public void DeleteWindowsThatNeedTo()
        {
            for (int i = 0; i < indexToDelete.Count; i++)
            {
                Node b = GetNodeWithIndex(indexToDelete[i]);
                if(b != null)
					nodes.Remove(b);
            }

            indexToDelete.Clear();
        }

        public void DeleteNode(int index)
        {
			if(!indexToDelete.Contains(index))
				indexToDelete.Add(index);
        }

        public bool IsStateDuplicate(Node b)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id == b.id)
                    continue;
                
				if (nodes[i].stateRef.currentState == b.stateRef.currentState &&
                    !nodes[i].isDuplicate)
                    return true;
            }
            return false;
        }

        public bool IsTransitionDuplicate(Node b)
        {
            Node enter = GetNodeWithIndex(b.enterNode);
            if (enter == null)
            {
                Debug.Log("false");
                return false;
            }
            for (int i = 0; i < enter.stateRef.currentState.transitions.Count; i++)
            {
                Transition t = enter.stateRef.currentState.transitions[i];
                if (t.condition == b.transRef.previousCondition && b.transRef.transitionId != t.id)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
