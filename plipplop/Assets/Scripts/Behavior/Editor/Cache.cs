using UnityEngine;
namespace Behavior.Editor
{
	public class Cache : ScriptableObject
	{
		public BehaviorGraph graph = null;
		public AIStateDrawNode stateNode = null;
		public TransitionDrawNode transitionNode = null;
	}
}
