using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior.Editor
{
    public abstract class DrawNode : ScriptableObject
    {
        public readonly static float handleSize = 7f;

        public abstract void DrawWindow(Node b);
        public virtual void DrawCurve(Node b)
        {
            Rect rect = b.windowRect.Shift(-BehaviorEditor.scrollPos);

            for (int i = 0; i < b.exitNodes.Count; i++) {
                if (b.exitNodes[i] == null) continue;


                Node targetNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(b.exitNodes[i]);

                List<Vector2> points = new List<Vector2>();


                Vector2 startPos = GetExitPosition(b, i);
                Vector2 endPos = GetEntryPosition(targetNode);

                points.Add(startPos - BehaviorEditor.scrollPos);
                // Reroute nodes
                foreach (var reroute in b.GetReroutes(i)) {
                    points.Add(reroute.position - BehaviorEditor.scrollPos);
                }

                points.Add(endPos - BehaviorEditor.scrollPos);

                Color targetColor = Color.white;

                if (i == 0 && b is AIStateTransitionNode) targetColor = Color.green;
                if (i == 1) targetColor = Color.red;

                for (int j = 1; j < points.Count; j++) {
                    BehaviorEditor.DrawConnection(points[j-1], points[j], targetColor);
                }
            }
        }

        public Vector2 GetExitPosition(Node b, int index = 0)
        {
            return new Vector2(b.windowRect.width + b.windowRect.x + handleSize * 1.5f, b.windowRect.y + b.windowRect.height / 4f + index * (b.windowRect.height / 4f));
        }

        public Vector2 GetEntryPosition(Node b)
        {
            return new Vector2(b.windowRect.x - handleSize * 1.5f, b.windowRect.y + b.windowRect.height / 2f);
        }
    }
}
