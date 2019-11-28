using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behavior
{
    namespace Editor
    {
        public class BehaviorEditor : EditorWindow
        {
            #region Variables
            Vector3 mousePosition;
            bool clickedOnWindow;
            Node selectedNode;
            float zoom = 1f;

            public static Settings settings;
            public static readonly int startNodeId = -1;

            int transitFromId;
            Rect mouseRect = new Rect(0, 0, 1, 1);
            Rect all = new Rect(-5, -5, 10000, 10000);
            GUIStyle style;
            GUIStyle activeStyle;
            Vector2 scrollPos;
            Vector2 scrollStartPos;
            static BehaviorEditor editor;
            static AIState previousAIState;
            int nodesToDelete;

            public enum EUserActions
            {
                ADD_STATE,
                ADD_TRANSITION_NODE,
                DELETE_NODE,
                COMMENT_NODE,
                MAKE_TRANSITION_TRUE,
                MAKE_TRANSITION_FALSE,
                RESET_PAN
            }
            #endregion

            #region Init
            [MenuItem("Window/Behavior Editor")]
            static void ShowEditor()
            {
                editor = EditorWindow.GetWindow<BehaviorEditor>();
                editor.minSize = new Vector2(800, 600);

            }

            private void OnEnable()
            {
                settings = Resources.Load("Settings") as Settings;
                style = settings.skin.GetStyle("window");
                activeStyle = settings.activeSkin.GetStyle("window");

            }
            #endregion

            private void Update()
            {
                // Safety check 
                if (settings.currentGraph.GetNodeWithIndex(startNodeId) == null) {
                    settings.currentGraph.initialState.SetGraph(settings.currentGraph);
                    var node = (AIStateNode)settings.AddNodeOnGraph(settings.stateNode, 300, 64, "START", Vector3.one*0.5f);
                    node.id = startNodeId;
                    node.currentAIState = settings.currentGraph.initialState;
                    if (!settings.currentGraph.initialState) {
                        Debug.LogError("No initial state set for " + settings.currentGraph + ", this should be fixed urgently.");
                    }
                }
                else {
                    // Making sure we have only 1 start node
                    bool pastFirstNode = false;
                    foreach(var node in settings.currentGraph.nodes.ToArray()) {
                        if (node.id == startNodeId) {
                            if (!pastFirstNode) {
                                pastFirstNode = true;
                                continue;
                            }
                            else {
                                settings.currentGraph.DeleteNode(node.id);
                            }
                        }
                    }
                }

                // Updating all nodes

                foreach (var node in settings.currentGraph.nodes.ToArray()) {
                    node.SetGraph(settings.currentGraph);
                }
                /*
                if (currentAIStateManager != null)
                {
                    if (previousAIState != currentAIStateManager.currentAIState)
                    {
                        Repaint();
                        previousAIState = currentAIStateManager.currentAIState;
                    }
                }
                */

                if (settings.currentGraph.AreSomeNodesPendingDeletion()) {
                if (settings.currentGraph != null) {
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                    Debug.Log("Deleting " + nodesToDelete + " windows that needs to");
                    Repaint();
                }
                nodesToDelete = 0;
                }
            }

            #region GUI Methods
            private void OnGUI()
            {
                /*
                if (Selection.activeTransform != null)
                {
                    currentAIStateManager = Selection.activeTransform.GetComponentInChildren<AIStateManager>();
                    if (prevAIStateManager != currentAIStateManager)
                    {
                        prevAIStateManager = currentAIStateManager;
                        Repaint();
                    }
                }
                */

                Event e = Event.current;
                mousePosition = e.mousePosition;
                UserInput(e);

                DrawWindows();

                if (e.type == EventType.MouseDrag) {
                    if (settings.currentGraph != null) Repaint();
                }

                if (GUI.changed) {
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                }

                if (settings.MAKE_TRANSITION) {
                    mouseRect.x = mousePosition.x;
                    mouseRect.y = mousePosition.y;
                    Rect from = settings.currentGraph.GetNodeWithIndex(transitFromId).windowRect;
                    DrawNodeCurve(from, mouseRect, true, Color.blue);
                    Repaint();
                }

            }

            void DrawWindows()
            {
                GUILayout.BeginArea(all, style);
                BeginWindows();
                EditorGUILayout.LabelField("Assign Graph:", GUILayout.Width(100));
                settings.currentGraph = (BehaviorGraph)EditorGUILayout.ObjectField(settings.currentGraph, typeof(BehaviorGraph), false, GUILayout.Width(200));
                if (settings.currentGraph != null) {

                    for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                        Node b = settings.currentGraph.nodes[i];
                        if (b.drawNode is AIStateDrawNode) {
                            var bS = (AIStateNode)b;
                            if (bS.currentAIState != null && bS.currentAIState.id == settings.currentGraph.GetCurrentAIStateID()) {
                                b.windowRect = GUI.Window(i, b.windowRect, DrawNodeWindow, b.windowTitle, activeStyle);
                            }
                            else {
                                b.windowRect = GUI.Window(i, b.windowRect, DrawNodeWindow, b.windowTitle);
                            }
                        }
                        else {
                            b.windowRect = GUI.Window(i, b.windowRect, DrawNodeWindow, b.windowTitle);
                        }
                    }
                    foreach (Node n in settings.currentGraph.nodes) n.DrawCurve();
                }
                EditorGUILayout.LabelField(string.Format("x:{0} y:{1}", scrollPos.x, scrollPos.y));
                EndWindows();
                GUILayout.EndArea();
            }

            void DrawNodeWindow(int id)
            {
                settings.currentGraph.nodes[id].DrawWindow();
                GUI.DragWindow();
            }

            void UserInput(Event e)
            {
                if (settings.currentGraph == null) return;

                if (e.button == 0) // LEFT CLICK
                {
                    if (e.type == EventType.MouseDown) {
                        if (settings.MAKE_TRANSITION) {
                            if (settings.TRANSITION_TYPE) MakeTransitionIfTrue();
                            else MakeTransitionIfFalse();
                        }
                    }
                }
                else if (e.button == 1) // RIGHT CLICK
                {
                    if (e.type == EventType.MouseDown) {
                        if (!settings.MAKE_TRANSITION) RightClick(e);
                    }
                }
                else if (e.button == 2) // MIDDLE CLICK
                {
                    if (e.type == EventType.MouseDown) {
                        scrollStartPos = e.mousePosition;
                    }
                    else if (e.type == EventType.MouseDrag) {
                        HandlePanning(e);
                    }
                }
            }

            void HandlePanning(Event e)
            {
                Vector2 diff = e.mousePosition - scrollStartPos;
                diff *= .6f;
                scrollStartPos = e.mousePosition;
                scrollPos += diff;

                for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                    Node b = settings.currentGraph.nodes[i];
                    b.windowRect.x += diff.x;
                    b.windowRect.y += diff.y;
                }
            }

            void ResetScroll()
            {
                for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                    Node b = settings.currentGraph.nodes[i];
                    b.windowRect.x -= scrollPos.x;
                    b.windowRect.y -= scrollPos.y;
                }

                scrollPos = Vector2.zero;
            }

            void RightClick(Event e)
            {
                clickedOnWindow = false;
                for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                    if (settings.currentGraph.nodes[i].windowRect.Contains(e.mousePosition)) {
                        clickedOnWindow = true;
                        selectedNode = settings.currentGraph.nodes[i];
                        break;
                    }
                }

                if (!clickedOnWindow) AddNewNode(e);
                else ModifyNode(e);
            }

            void MakeTransition(System.Action<Node, Node> setTargetAndId)
            {
                settings.MAKE_TRANSITION = false;
                clickedOnWindow = false;
                for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                    if (settings.currentGraph.nodes[i].windowRect.Contains(mousePosition)) {
                        clickedOnWindow = true;
                        selectedNode = settings.currentGraph.nodes[i];
                        break;
                    }
                }

                if (clickedOnWindow) {
                    if (selectedNode.id != transitFromId) {
                        Node transitionOriginNode = settings.currentGraph.GetNodeWithIndex(transitFromId);

                        if (selectedNode == null) {
                            Debug.LogWarning("I cannot create a transition to an empty node!");
                        }
                        else {
                            setTargetAndId.Invoke(transitionOriginNode, selectedNode) ;
                        }
                    }
                }
            }

            void MakeTransitionIfFalse()
            {
                MakeTransition((transitionNode, targetNode) => {
                    transitionNode.exitNodes[1] = targetNode.id;
                });
            }

            void MakeTransitionIfTrue()
            {
                MakeTransition((transitionNode, targetNode) => {
                    transitionNode.exitNodes[0] = targetNode.id;
                });
            }
            #endregion

            #region Context Menus
            void AddNewNode(Event e)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                if (settings.currentGraph != null) {
                    menu.AddItem(new GUIContent("Add AIState"), false, ContextCallback, EUserActions.ADD_STATE);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, EUserActions.RESET_PAN);
                }
                else {
                    menu.AddDisabledItem(new GUIContent("Add AIState"));
                    menu.AddDisabledItem(new GUIContent("Add Comment"));
                }
                menu.ShowAsContext();
                e.Use();
            }

            void ModifyNode(Event e)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                if (selectedNode.drawNode is AIStateDrawNode) {
                    if (((AIStateNode)selectedNode).currentAIState != null) {
                        menu.AddItem(new GUIContent("Add condition"), false, ContextCallback, EUserActions.ADD_TRANSITION_NODE);
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Add condition"));
                    }
                    menu.AddItem(new GUIContent("Delete"), false, ContextCallback, EUserActions.DELETE_NODE);
                }
                else if (selectedNode.drawNode is TransitionDrawNode) {
                    if (selectedNode.isDuplicate || !selectedNode.isAssigned) {
                        menu.AddDisabledItem(new GUIContent("Make transition..."));
                    }
                    else {
                        menu.AddItem(new GUIContent("Make transition when TRUE..."), false, ContextCallback, EUserActions.MAKE_TRANSITION_TRUE);
                        menu.AddItem(new GUIContent("Make transition when FALSE..."), false, ContextCallback, EUserActions.MAKE_TRANSITION_FALSE);
                    }
                    menu.AddItem(new GUIContent("Delete"), false, ContextCallback, EUserActions.DELETE_NODE);
                }
                menu.ShowAsContext();
                e.Use();
            }

            void ContextCallback(object o)
            {
                EUserActions a = (EUserActions)o;
                switch (a) {
                    case EUserActions.ADD_STATE:
                        settings.AddNodeOnGraph(settings.stateNode, 300, 200, "State", mousePosition);
                        break;
                    case EUserActions.ADD_TRANSITION_NODE:
                        AddTransitionNode((AIStateNode)selectedNode, mousePosition);
                        break;
                    default:
                        break;
                    case EUserActions.DELETE_NODE:
                        if (selectedNode.drawNode is TransitionDrawNode) {
                            Node enterNode = settings.currentGraph.GetNodeWithIndex(selectedNode.enterNode);
                            if (enterNode != null) {
                                if (enterNode is AIStateNode) {
                                    ((AIStateNode)enterNode).RemoveTransition();
                                }
                                else {
                                    var transitionNode = ((AIStateTransitionNode)enterNode);
                                    for (int i = 0; i < transitionNode.exitNodes.Length; i++) {
                                        if (transitionNode.exitNodes[i] == selectedNode.id) {
                                            transitionNode.exitNodes[i] = null;
                                        }
                                    }
                                }
                            }
                        }

                        nodesToDelete++;
                        settings.currentGraph.DeleteNode(selectedNode.id);
                        break;
                    case EUserActions.MAKE_TRANSITION_TRUE:
                        transitFromId = selectedNode.id;
                        settings.MAKE_TRANSITION = true;
                        settings.TRANSITION_TYPE = true;
                        break;
                    case EUserActions.MAKE_TRANSITION_FALSE:
                        transitFromId = selectedNode.id;
                        settings.MAKE_TRANSITION = true;
                        settings.TRANSITION_TYPE = false;
                        break;
                    case EUserActions.RESET_PAN:
                        ResetScroll();
                        break;
                }
                // Set dirty here
            }

            public static Node AddTransitionNode(AIStateNode originNode, Vector3 pos)
            {
                Node transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 50, "Condition", pos);
                transNode.enterNode = originNode.id;
                AIStateTransitionNode t = settings.currentGraph.AddTransition(originNode);
                return transNode;
            }
            #endregion

            #region Helper Methods
            public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
            {
                Vector3 startPos = GetBestArrowPosition(start, end);
                Vector3 endPos = GetBestArrowPosition(end, start);
                Color c = curveColor;

                Vector3 dir = (startPos - endPos).normalized;
                Vector3 right = Quaternion.AngleAxis(-90, Vector3.forward) * dir;
                Handles.DrawAAConvexPolygon(new Vector3[] { startPos - right * 2, startPos + right * 2, endPos - right * 2, endPos + right * 2 });
                Vector3 center = (startPos + endPos) * 0.5f;
                DrawTriangle(center, -dir, 10, c);
            }

            public static Vector3 GetBestArrowPosition(Rect start, Rect end)
            {
                float posx = 0;
                float posy = 0;
                // IS INSIDE WIDTH
                if (start.center.x <= end.center.x + end.width / 2 && start.center.x >= end.center.x - end.width / 2) {
                    posx = start.center.x;
                }
                else {
                    if (start.center.x >= end.center.x + end.width / 2) posx = start.position.x;
                    else if (start.center.x <= end.center.x - end.width / 2) posx = start.position.x + start.width;
                }

                // IS INSIDE HEIGHT
                if (start.center.y <= end.center.y + end.height / 2 && start.center.y >= end.center.y - end.height / 2) {
                    posy = start.center.y;
                }
                else {
                    if (start.center.y >= end.center.y + end.height / 2) posy = start.position.y;
                    else if (start.center.y <= end.center.y - end.height / 2) posy = start.position.y + start.height;
                }
                return new Vector3(posx, posy, 0f);
            }

            public static void DrawTriangle(Vector3 position, Vector3 direction, float size, Color color, bool pivotOnPointyEdge = false)
            {
                Handles.color = color;
                Vector3 right = Quaternion.AngleAxis(-90, Vector3.forward) * direction;
                if (!pivotOnPointyEdge) {
                    Handles.DrawAAConvexPolygon(
                        new Vector3[]{
                        position - right * size,
                        position + right * size,
                        position + direction * size * 2
                        }
                    );

                }
                else {
                    Handles.DrawAAConvexPolygon(
                        new Vector3[]{
                        position,
                        position - (direction * size * 2) + right * size,
                        position - (direction * size * 2) - right * size,
                        }
                    );
                }
            }

            public static void ClearWindowsFromList(List<Node> l)
            {
                for (int i = 0; i < l.Count; i++) {
                    //      if (windows.Contains(l[i]))
                    //        windows.Remove(l[i]);
                }
            }

            #endregion
        }
    }
}
