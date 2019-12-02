using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.Editor
{
    public abstract class DrawNode : ScriptableObject
    {
        public abstract void DrawWindow(Node b);
        public virtual void DrawCurve(Node b)
        {
            Rect rect = b.windowRect.Shift(-BehaviorEditor.scrollPos);

            for (int i = 0; i < b.exitNodes.Count; i++) {
                if (b.exitNodes[i] == null) continue;

                Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);
                if (targetNode != null) {
                    rect = b.windowRect.Shift(-BehaviorEditor.scrollPos);
                    //rect.x += rect.width;
                    Rect endRect = targetNode.windowRect.Shift(-BehaviorEditor.scrollPos);
                    //endRect.x -= endRect.width * .5f;

                    Color targetColor = Color.white;

                    if (i == 0 && b is AIStateTransitionNode) targetColor = Color.green;
                    if (i == 1) targetColor = Color.red;

                    //rect.position = new Vector2(rect.position.x - rect.size.x, rect.position.y);
                    BehaviorEditor.DrawNodeCurve(rect, endRect, targetColor, i);
                }
            }
        }
    }
}
