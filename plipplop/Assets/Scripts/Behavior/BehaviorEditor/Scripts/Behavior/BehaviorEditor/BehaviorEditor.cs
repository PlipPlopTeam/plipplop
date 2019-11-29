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

            public static Settings settings;
            public static float zoom = 1f;
            public static readonly int startNodeId = -1;

            readonly float minWindowSize = 50f;
            readonly float minimapSize = 64f;
            readonly float minimapMargin = 5f;
            readonly float highlightWidth = 5f;
            Texture2D minimapTexture;

            int transitFromId;
            Rect mouseRect = new Rect(0, 0, 1, 1);
            readonly Rect all = new Rect(0, 0, 5000, 5000);

            GUIStyle style;
            GUIStyle activeStyle;
            public static Vector2 scrollPos;
            Vector2 scrollStartPos;
            static BehaviorEditor editor;
            int nodesToDelete;
            SerializedObject serializedGraph;

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
                serializedGraph = new SerializedObject(settings.currentGraph);
                minimapTexture = new Texture2D((int)minimapSize, (int)minimapSize);
                scrollPos = settings.currentGraph.editorScrollPosition;

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
                    foreach(var node in settings.currentGraph.nodes) {
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

                foreach (var node in settings.currentGraph.nodes) {
                    node.SetGraph(settings.currentGraph);
                    node.windowRect.size = new Vector2(Mathf.Max(node.windowRect.size.x, minWindowSize), Mathf.Max(node.windowRect.size.y, minWindowSize));
                    node.windowRect.position = new Vector2(
                          Mathf.Clamp(node.windowRect.x, 0f, all.width),
                          Mathf.Clamp(node.windowRect.y, 0f, all.height)
                    );
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
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                    Debug.Log("Deleting " + nodesToDelete + " windows that needs to");
                    Repaint();
                    nodesToDelete = 0;
                }

                settings.currentGraph.editorScrollPosition = scrollPos;
            }

            #region GUI Methods
            void OnGUI()
            {
                serializedGraph.Update();

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
                mousePosition = e.mousePosition + scrollPos;

                UserInput(e);

                DrawWindows();
                DrawMiniMap();

                if (e.type == EventType.MouseDrag) {
                    if (settings.currentGraph != null) Repaint();
                }

                if (GUI.changed) {
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                }

                if (settings.MAKE_TRANSITION) {
                    Rect from = settings.currentGraph.GetNodeWithIndex(transitFromId).windowRect.Shift(-scrollPos);
                    DrawNodeCurve(from, new Rect(mousePosition, Vector2.zero).Shift(-scrollPos), true, Color.blue);
                    Repaint();
                }

                SaveChanges();
            }

            void SaveChanges()
            {
            //    Debug.Log("SAVING ========");
                var newSG = new SerializedObject(settings.currentGraph);
                var it = newSG.GetIterator();
                for(; ;) {
                    try {
                        if (!it.NextVisible(true) && !it.NextVisible(false)) {
                            break;
                        }
                    }
                    catch (System.InvalidOperationException) {
                        break;
                    }
                    if (serializedGraph.FindProperty(it.propertyPath) != null) {
            //            Debug.Log("Saving (if different) " + it.propertyPath);
                        serializedGraph.CopyFromSerializedPropertyIfDifferent(it);
                    } 
                }
                serializedGraph.ApplyModifiedProperties();
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
                                b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle+":"+b.id, activeStyle).Shift(scrollPos);
                            }
                            else {
                                b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle + ":" + b.id).Shift(scrollPos);
                            }
                        }
                        else {
                            b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle + ":" + b.id).Shift(scrollPos);
                        }
                    }
                    foreach (Node n in settings.currentGraph.nodes) n.DrawCurve();
                }
                EditorGUILayout.LabelField(string.Format("x:{0} y:{1} zoom:{2}", scrollPos.x, scrollPos.y, zoom));
                EndWindows();
                GUILayout.EndArea();
            }

            void DrawMiniMap()
            {
                var rect = new Rect(new Vector2(minimapMargin, Screen.height - minimapMargin - minimapSize - 20), new Vector2(minimapSize, minimapSize));
                
                for (int x = 0; x < minimapTexture.width; x++) {
                    for (int y = 0; y < minimapTexture.height; y++) {
                        minimapTexture.SetPixel(x, y, new Color(1f, 1F, 1f, 0.2f));
                    }
                }
                
                minimapTexture.SetPencilColor(Color.black);


                foreach (var node in settings.currentGraph.nodes) {
                    var position = new Vector2(
                        (node.windowRect.x / all.width),
                        1f - (node.windowRect.y / all.height)
                    );
                    var wSizeUV = new Vector2(node.windowRect.width / all.width, node.windowRect.width / all.height);
                    try {
                        minimapTexture.SetPencilColor(Color.black);
                        minimapTexture.Polygon(
                            new List<Vector2>() {
                                position,
                                new Vector2(position.x+wSizeUV.x, position.y),
                                new Vector2(position.x+wSizeUV.x, position.y - wSizeUV.y),
                                new Vector2(position.x, position.y-wSizeUV.y)
                            }, 0.2f
                        );
                    }
                    catch (Pencil.OutOfUVException) {
                        // Ignore
                    }
                }

                var viewPos = Vector2.one - new Vector2(
                    1f - (scrollPos.x / all.width),
                    (scrollPos.y / all.height)
                );
                var sizeUV = new Vector2(Screen.width/all.width, Screen.height/all.height);

                minimapTexture.SetPencilColor(Color.white);
                minimapTexture.Polygon(new List<Vector2>() {
                    viewPos, 
                    new Vector2(viewPos.x+sizeUV.x, viewPos.y),
                    new Vector2(viewPos.x+sizeUV.x, viewPos.y - sizeUV.y), 
                    new Vector2(viewPos.x, viewPos.y-sizeUV.y)
                }, 0.1f);

                minimapTexture.Apply();
                GUI.DrawTexture(rect, minimapTexture);
            }

            void DrawNodeWindow(int id)
            {
                settings.currentGraph.nodes[id].DrawWindow();
                GUI.DragWindow();
            }

            void UserInput(Event e)
            {
                if (settings.currentGraph == null) return;

                if (e.isScrollWheel) {
                    zoom += Mathf.Sign(e.delta.y) / 10f;
                } 
                else if (e.button == 0) // LEFT CLICK
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
                scrollPos -= diff;

                scrollPos = new Vector2(
                    Mathf.Clamp(scrollPos.x, 0f, all.width),
                    Mathf.Clamp(scrollPos.y, 0f, all.height)
                );
            }

            void ResetScroll()
            {
                scrollPos = Vector2.Scale(Vector2.one*0.5f, all.size);
            }

            void RightClick(Event e)
            {
                clickedOnWindow = false;
                for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                    if (settings.currentGraph.nodes[i].windowRect.Contains(mousePosition)) {
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
                    transitionNode.SetExitNode(1, targetNode.id);
                });
            }

            void MakeTransitionIfTrue()
            {
                MakeTransition((transitionNode, targetNode) => {
                    transitionNode.SetExitNode(0, targetNode.id);
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
                                    for (int i = 0; i < transitionNode.exitNodes.Count; i++) {
                                        if (transitionNode.exitNodes[i] == selectedNode.id) {
                                            transitionNode.RemoveExitNode(i);
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
                //
                startPos = start.position + start.size / 2f;
                endPos = end.position + end.size / 2f;
                //
                Color c = curveColor;
                Handles.color = c;
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
