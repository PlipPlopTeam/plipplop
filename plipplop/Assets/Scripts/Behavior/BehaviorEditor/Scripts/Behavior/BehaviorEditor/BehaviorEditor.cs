﻿using System.Collections;
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

            readonly static float minWindowSize = 50f;
            readonly static float minimapSize = 64f;
            readonly static float minimapMargin = 5f;
            readonly static float rerouteDragDistance = 30f;

            Texture2D minimapTexture;

            int transitFromId;
            int currentTransitionSlotIndex = 0;
            Rect mouseRect = new Rect(0, 0, 1, 1);
            readonly Rect all = new Rect(0, 0, 5000, 5000);

            GUIStyle style;
            GUIStyle activeStyle;
            public static Vector2 scrollPos;
            Vector2 scrollStartPos;
            static BehaviorEditor editor;
            int nodesToDelete;
            Node.Reroute draggedReroute = null;
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
                style = settings.skin.window;
                activeStyle = settings.activeSkin.window; 
                serializedGraph = new SerializedObject(settings.currentGraph);
                minimapTexture = new Texture2D((int)minimapSize, (int)minimapSize);
                scrollPos = settings.currentGraph.editorScrollPosition;

            }
            #endregion

            private void Update()
            {               
                // Safety check 
                if (settings.currentGraph.GetNodeWithIndex(startNodeId) == null) {
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

                if (settings.currentGraph.AreSomeNodesPendingDeletion()) {
                    settings.currentGraph.DeleteWindowsThatNeedTo();
                    Repaint();
                    nodesToDelete = 0;
                }

                settings.currentGraph.editorScrollPosition = scrollPos;
            }

            #region GUI Methods
            void OnGUI()
            { 
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
                }

                if (settings.MAKE_TRANSITION) {
                    var nodeFrom = settings.currentGraph.GetNodeWithIndex(transitFromId);
                    var from = nodeFrom.drawNode.GetExitPosition(nodeFrom, currentTransitionSlotIndex);

                    DrawConnection(from - scrollPos, new Vector2(mousePosition.x, mousePosition.y) -scrollPos, nodeFrom is AIStateNode ? Color.white : settings.TRANSITION_TYPE ? Color.green : Color.red);
                }

                Repaint();
            }
            
            void SaveChanges()
            {
                //Debug.Log("SAVING ========");
                var newSG = new SerializedObject(settings.currentGraph);
                var it = newSG.GetIterator();

                for (; ;) {
                    try {
                        if (!it.NextVisible(true) && !it.NextVisible(false)) {
                            //Debug.Log("Nothing visible, breaking");
                            break;
                        }
                    }
                    catch (System.InvalidOperationException e) {
                        //Debug.Log("Invalid operation, stopped at "+e.ToString());
                        break;
                    }
                    //Debug.Log("Found property " + it.propertyPath);
                    if (serializedGraph.FindProperty(it.propertyPath) != null) {
                        //Debug.Log("Saving (if different) " + it.propertyPath);
                        serializedGraph.CopyFromSerializedPropertyIfDifferent(it);
                    }
                    //Debug.Log("Next...");
                }
                serializedGraph.ApplyModifiedProperties();
                serializedGraph.Update();

                // 😭
                EditorUtility.SetDirty(settings.currentGraph);
            }

            void DrawWindows()
            {
                GUILayout.BeginArea(all, style);
                BeginWindows();
                EditorGUILayout.LabelField("Assign Graph:", GUILayout.Width(100));
                settings.currentGraph = (BehaviorGraph)EditorGUILayout.ObjectField(settings.currentGraph, typeof(BehaviorGraph), false, GUILayout.Width(200));
                if (settings.currentGraph != null) {
                    // Windows
                    for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                        Node b = settings.currentGraph.nodes[i];

                        // Out of screen ?
                        var shiftedPos = b.windowRect.Shift(-scrollPos);
                        var shiftedPosBottomRight = shiftedPos.Shift(b.windowRect.size);
                        if (
                            (shiftedPosBottomRight.x < 0 && shiftedPos.x < 0) ||
                            (shiftedPosBottomRight.y < 0 && shiftedPos.y < 0) ||

                            (shiftedPosBottomRight.x > Screen.width && shiftedPos.x > Screen.width) ||
                            (shiftedPosBottomRight.y > Screen.height && shiftedPos.y > Screen.height) 
                        ) {
                            continue;
                        }


                        b.RefreshRectSize(zoom);
                        if (b.windowRect.Contains(mousePosition)) {
                            b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle + ":" + b.id, activeStyle).Shift(scrollPos);
                        } 
                        else if (b.drawNode is AIStateDrawNode) {
                            var bS = (AIStateNode)b;
                            if (bS.currentAIState != null && bS.currentAIState.id == settings.currentGraph.GetCurrentAIStateID()){
                                b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle+":"+b.id, activeStyle).Shift(scrollPos);
                            }
                            else {
                                b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle + ":" + b.id, style).Shift(scrollPos);
                            }
                        }
                        else {
                            b.windowRect = GUI.Window(i, b.windowRect.Shift(-scrollPos), DrawNodeWindow, b.windowTitle + ":" + b.id, style).Shift(scrollPos);
                        }
                    }

                    // Handles
                    foreach (var node in settings.currentGraph.nodes) {
                        // Enter node
                        if (settings.MAKE_TRANSITION) {
                            if (Handles.Button(new Vector2(node.windowRect.x - DrawNode.handleSize * 1.5f, node.windowRect.y + node.windowRect.height/2f) - scrollPos, Quaternion.identity, DrawNode.handleSize, DrawNode.handleSize, ButtonCap)) {
                                var origin = settings.currentGraph.GetNodeWithIndex(transitFromId);
                                origin.RemoveExitNode(origin is AIStateNode || settings.TRANSITION_TYPE  ? 0 : 1);
                                if (settings.TRANSITION_TYPE) MakeTransitionIfTrue(node.id);
                                else MakeTransitionIfFalse(node.id);
                            }
                        }
                        else {
                            Handles.DotHandleCap(0, new Vector2(node.windowRect.x - DrawNode.handleSize * 1.5f, node.windowRect.y + node.windowRect.height / 2f) - scrollPos, Quaternion.LookRotation(Vector3.forward), DrawNode.handleSize, EventType.Repaint);
                        }

                        // Exit nodes
                        int i = 0;
                        foreach(var exit in node.exitNodes) {
                            var exitNodeHeight = node.drawNode.GetExitPosition(node, i);
                            Handles.color = node is AIStateNode ? Color.white : i == 0 ? Color.green : Color.red;
                            if (Handles.Button(new Vector2(node.windowRect.x + node.windowRect.width + DrawNode.handleSize * 1.5f, exitNodeHeight.y) - scrollPos, Quaternion.identity, DrawNode.handleSize, DrawNode.handleSize, ButtonCap)) {
                                settings.MAKE_TRANSITION = true;
                                settings.TRANSITION_TYPE = (i == 0);
                                transitFromId = node.id;
                                currentTransitionSlotIndex = i;
                            }
                            i++;
                        }
                    }

                    // Curves
                    foreach (Node n in settings.currentGraph.nodes) n.DrawCurve();
                }
                EditorGUILayout.LabelField(string.Format("x:{0} y:{1} fps:{2}", scrollPos.x, scrollPos.y, Mathf.Round(1f/Time.deltaTime)));
                EndWindows();
                GUILayout.EndArea();
            }

            void ButtonCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
            {
                Handles.DotHandleCap(controlID, position, Quaternion.LookRotation(Vector3.forward), DrawNode.handleSize, eventType);
            }

            void DrawMiniMap()
            {
                var rect = new Rect(new Vector2(minimapMargin, Screen.height - minimapMargin - minimapSize - 20), new Vector2(minimapSize, minimapSize));
                
                if (!minimapTexture) {
                    minimapTexture = new Texture2D((int)minimapSize, (int)minimapSize);
                }

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
				/*
                if (e.isScrollWheel) {
                    zoom += Mathf.Sign(e.delta.y) / 10f;
                } 
				*/
                else if (e.button == 0) // LEFT CLICK
                {
                    if (e.type == EventType.MouseDown) {
                        if (settings.MAKE_TRANSITION) {
                            if (settings.TRANSITION_TYPE) MakeTransitionIfTrue();
                            else MakeTransitionIfFalse();
                        }
                    }
                    if (e.type == EventType.MouseDrag) {
                        if (!settings.MAKE_TRANSITION) {
                            var mousePosition2 = new Vector2(mousePosition.x, mousePosition.y);
                            if (draggedReroute != null) {
                                draggedReroute.position = mousePosition2;
                            }
                            else {
                                foreach (var node in settings.currentGraph.nodes) {
                                    for (int i = 0; i < node.exitNodes.Count; i++) {
                                        foreach (var reroute in node.GetReroutes(i)) {
                                            if (Vector2.Distance(reroute.position, mousePosition2) < rerouteDragDistance) {
                                                draggedReroute = reroute;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (e.type == EventType.MouseUp) {
                        draggedReroute = null;
                    }
                }
                else if (e.button == 1) // RIGHT CLICK
                {
                    if (e.type == EventType.MouseDown) {
                        if (!settings.MAKE_TRANSITION) RightClick(e);
                        else settings.MAKE_TRANSITION = false;
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

            void MakeTransition(System.Action<Node, Node> setTargetAndId, int? forceSelection=null)
            {
                clickedOnWindow = false;
                if (forceSelection.HasValue) {
                    clickedOnWindow = true;
                    var nodes = new List<Node>(settings.currentGraph.nodes);
                    selectedNode = nodes.Find(o => o.id == forceSelection.Value);
                }
                else {
                    for (int i = 0; i < settings.currentGraph.nodes.Count; i++) {
                        if (settings.currentGraph.nodes[i].windowRect.Contains(mousePosition)) {
                            clickedOnWindow = true;
                            selectedNode = settings.currentGraph.nodes[i];
                            break;
                        }
                    }
                }

                if (clickedOnWindow) {
                    settings.MAKE_TRANSITION = false;
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

            void MakeTransitionIfFalse(int? forceTarget=null)
            {
                MakeTransition((transitionNode, targetNode) => {
                    transitionNode.SetExitNode(1, targetNode.id);
                }, forceTarget);
            }

            void MakeTransitionIfTrue(int? forceTarget = null)
            {
                MakeTransition((transitionNode, targetNode) => {
                    transitionNode.SetExitNode(0, targetNode.id);
                }, forceTarget);
            }
            #endregion

            #region Context Menus
            void AddNewNode(Event e)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                if (settings.currentGraph != null) {
                    menu.AddItem(new GUIContent("Add state"), false, ContextCallback, EUserActions.ADD_STATE);
                    menu.AddItem(new GUIContent("Add condition"), false, ContextCallback, EUserActions.ADD_TRANSITION_NODE);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, EUserActions.RESET_PAN);
                    menu.AddItem(new GUIContent("Save graph"), false, SaveChanges);
                }
                else {
                    menu.AddDisabledItem(new GUIContent("Add AIState"));
                    menu.AddDisabledItem(new GUIContent("Add condition"));
                }
                menu.ShowAsContext();
                e.Use();
            }

            void ModifyNode(Event e)
            {

                // Adding and removing links
                GenericMenu menu = new GenericMenu();
                menu.AddSeparator("");
                if (selectedNode is AIStateNode) {
                    menu.AddItem(new GUIContent("Add condition"), false, ContextCallback, EUserActions.ADD_TRANSITION_NODE);
                }
                else if (selectedNode is AIStateTransitionNode) {
                    menu.AddItem(new GUIContent("Make transition when TRUE..."), false, ContextCallback, EUserActions.MAKE_TRANSITION_TRUE);
                    menu.AddItem(new GUIContent("Make transition when FALSE..."), false, ContextCallback, EUserActions.MAKE_TRANSITION_FALSE);
                }

                // REroutes
                menu.AddSeparator("");
                for (int i = 0; i < selectedNode.exitNodes.Count; i++) {
                    int index = i;
                    var exit = selectedNode.exitNodes[index];
                    var exitNode = settings.currentGraph.GetNodeWithIndex(exit);
                    if (exit.HasValue) {
                        menu.AddItem(new GUIContent("Add a reroute on output " + index.Letter()), false, delegate {
                            selectedNode.AddReroute(index).position = (exitNode.windowRect.position + selectedNode.windowRect.position) * 0.5f;
                        }); 

                        // Break reroutes
                        foreach(var reroute in selectedNode.GetReroutes(index)) {
                            menu.AddItem(new GUIContent("Delete reroute "+ reroute.beaconIndex.Letter() + " on output " + index.Letter()), false, delegate {
                                selectedNode.DeleteReroute(index, reroute.beaconIndex);
                            });
                        }
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Add a reroute on output " + index.Letter()));
                    }

                }

                // Break
                menu.AddSeparator("");
                for (int i = 0; i < selectedNode.exitNodes.Count; i++) {
                    int index = i;
                    var exit = selectedNode.exitNodes[index];
                    if (exit.HasValue) {
                        menu.AddItem(new GUIContent("Break output " + index.Letter()), false, delegate {
                            BreakOutput(selectedNode, index);
                        });
                    }
                    else {
                        menu.AddDisabledItem(new GUIContent("Break output " + index.Letter()));
                    }
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, EUserActions.DELETE_NODE);
                menu.ShowAsContext();
                e.Use();
            }

            void BreakOutput(Node n, int exit)
            {
                n.RemoveExitNode(exit);
            }

            void ContextCallback(object o)
            {
                EUserActions a = (EUserActions)o;
                switch (a) {
                    case EUserActions.ADD_STATE:
                        settings.AddNodeOnGraph(settings.stateNode, 300, 200, "State", mousePosition);
                        break;
                    case EUserActions.ADD_TRANSITION_NODE:
                        AddTransitionNode(selectedNode is AIStateNode ? (AIStateNode)selectedNode : null, mousePosition);
                        break;
                    default:
                        break;
                    case EUserActions.DELETE_NODE:
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

            public static Node AddTransitionNode(Vector3 pos)
            {
                return AddTransitionNode(null, pos);
            }

            public static Node AddTransitionNode(AIStateNode originNode, Vector3 pos)
            {
                Node transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 50, "Condition", pos);
                if (originNode != null) {
                    AIStateTransitionNode t = settings.currentGraph.AddTransition(originNode);
                }
                return transNode;
            }
            #endregion

            #region Helper Methods
            public static void DrawConnection(Vector3 start, Vector3 end, Color curveColor)
            {
                Color c = curveColor;
                Handles.color = c;
                Vector3 dir = (start - end).normalized;
                Vector3 right = Quaternion.AngleAxis(-90, Vector3.forward) * dir;
                Handles.DrawAAConvexPolygon(new Vector3[] { start - right * 2, start + right * 2, end - right * 2, end + right * 2 });
                Vector3 center = (start + end) * 0.5f;
                DrawTriangle(center, -dir, 10, c);
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
