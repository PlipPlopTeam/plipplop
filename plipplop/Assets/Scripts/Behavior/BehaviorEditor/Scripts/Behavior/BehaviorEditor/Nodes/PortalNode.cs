using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Behavior.Editor
{
	public class PortalNode : DrawNode
	{

		public override void DrawCurve(Node b)
		{

		}

		public override void DrawWindow(Node b)
		{
			b.stateRef.currentAIState = (AIState)EditorGUILayout.ObjectField(b.stateRef.currentAIState, typeof(AIState), false);
			b.isAssigned = b.stateRef.currentAIState != null;

			if (b.stateRef.previousAIState != b.stateRef.currentAIState)
			{
				b.stateRef.previousAIState = b.stateRef.currentAIState;
				BehaviorEditor.forceSetDirty = true;
			}
		}
	}
}
