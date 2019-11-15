using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PP.Behavior
{
    public class BehaviorEditor : EditorWindow
    {
        #region Variables
        Vector3 mousePosition;
        bool clickedOnWindow;
        Node selectedNode;

        public static Settings settings;
        int transitFromId;
        Rect mouseRect = new Rect(0, 0, 1, 1);
        Rect all = new Rect(-5, -5, 10000, 10000);
        GUIStyle style;
		GUIStyle activeStyle;
		Vector2 scrollPos;
		Vector2 scrollStartPos;
		static BehaviorEditor editor;
		public static StateManager currentStateManager;
		public static bool forceSetDirty;
		static StateManager prevStateManager;
		static State previousState;
		int nodesToDelete;

		public enum UserActions
        {
            addState,addTransitionNode,deleteNode,commentNode,makeTransition,makePortal,resetPan
        }
        #endregion

        #region Init
        [MenuItem("Behavior Editor/Open")]
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
			if (currentStateManager != null)
			{
				if (previousState != currentStateManager.currentState)
				{
					Repaint();
					previousState = currentStateManager.currentState;
				}
			}

			if (nodesToDelete > 0)
			{
				if (settings.currentGraph != null)
				{		
					settings.currentGraph.DeleteWindowsThatNeedTo();
					Repaint();
				}
				nodesToDelete = 0;
			}
		}

		#region GUI Methods
		private void OnGUI()
        {
			if (Selection.activeTransform != null)
			{
				currentStateManager = Selection.activeTransform.GetComponentInChildren<StateManager>();
				if (prevStateManager != currentStateManager)
				{
					prevStateManager = currentStateManager;
					Repaint();
				}
			}

			Event e = Event.current;
            mousePosition = e.mousePosition;
            UserInput(e);

            DrawWindows();

			if (e.type == EventType.MouseDrag)
			{
				if (settings.currentGraph != null) Repaint();
			}

			if (GUI.changed)
			{
				settings.currentGraph.DeleteWindowsThatNeedTo();
				Repaint();
			}

            if(settings.makeTransition)
            {
                mouseRect.x = mousePosition.x;
                mouseRect.y = mousePosition.y;
                Rect from = settings.currentGraph.GetNodeWithIndex(transitFromId).windowRect;
                DrawNodeCurve(from, mouseRect, true, Color.blue);
                Repaint();
            }

			if (forceSetDirty)
			{
				forceSetDirty = false;
				EditorUtility.SetDirty(settings);
				EditorUtility.SetDirty(settings.currentGraph);

				for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
				{
					Node n = settings.currentGraph.nodes[i];
					if(n.stateRef.currentState != null)
						EditorUtility.SetDirty(n.stateRef.currentState);
				}
			}
		}

		void DrawWindows()
        {
			GUILayout.BeginArea(all, style);
			BeginWindows();
            EditorGUILayout.LabelField("Assign Graph:", GUILayout.Width(100));
            settings.currentGraph = (BehaviorGraph)EditorGUILayout.ObjectField(settings.currentGraph, typeof(BehaviorGraph), false, GUILayout.Width(200));
			if (settings.currentGraph != null)
            {
                foreach (Node n in settings.currentGraph.nodes) n.DrawCurve();

                for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
                {
					Node b = settings.currentGraph.nodes[i];

					if (b.drawNode is StateNode)
					{
						if (currentStateManager != null && b.stateRef.currentState == currentStateManager.currentState)
						{
							b.windowRect = GUI.Window(i, b.windowRect,
								DrawNodeWindow, b.windowTitle,activeStyle);
						}
						else
						{
							b.windowRect = GUI.Window(i, b.windowRect,
								DrawNodeWindow, b.windowTitle);
						}
					}
					else
					{
						b.windowRect = GUI.Window(i, b.windowRect,
							DrawNodeWindow, b.windowTitle);
					}
                }
            }
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
				if (e.type == EventType.MouseDown)
				{
					if (settings.makeTransition) MakeTransition();
				}
			}
			else if (e.button == 1) // RIGHT CLICK
			{
				if (e.type == EventType.MouseDown)
				{
					if (!settings.makeTransition) RightClick(e);
				}
			}
			else if (e.button == 2) // MIDDLE CLICK
			{
				if (e.type == EventType.MouseDown)
				{
					scrollStartPos = e.mousePosition;
				}
				else if (e.type == EventType.MouseDrag)
				{
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

			for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
			{
				Node b = settings.currentGraph.nodes[i];
				b.windowRect.x += diff.x;
				b.windowRect.y += diff.y;
			}
		}

		void ResetScroll()
		{
			for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
			{
				Node b = settings.currentGraph.nodes[i];
				b.windowRect.x -= scrollPos.x;
				b.windowRect.y -= scrollPos.y;
			}

			scrollPos = Vector2.zero;
		}

        void RightClick(Event e)
        {
            clickedOnWindow = false;
            for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
            {
                if (settings.currentGraph.nodes[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = settings.currentGraph.nodes[i];
                    break;
                }
            }
			 
            if(!clickedOnWindow) AddNewNode(e);
            else ModifyNode(e);
        }
       
        void MakeTransition()
        {
            settings.makeTransition = false;
            clickedOnWindow = false;
            for (int i = 0; i < settings.currentGraph.nodes.Count; i++)
            {
                if (settings.currentGraph.nodes[i].windowRect.Contains(mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = settings.currentGraph.nodes[i];
                    break;
                }
            }

            if(clickedOnWindow)
            {
                if(selectedNode.drawNode is StateNode || selectedNode.drawNode is PortalNode)
                {
                    if(selectedNode.id != transitFromId)
                    {
                        Node transNode = settings.currentGraph.GetNodeWithIndex(transitFromId);
                        transNode.targetNode = selectedNode.id;

                        Node enterNode = BehaviorEditor.settings.currentGraph.GetNodeWithIndex(transNode.enterNode);
                        Transition transition = enterNode.stateRef.currentState.GetTransition(transNode.transRef.transitionId);

						transition.targetState = selectedNode.stateRef.currentState;
                    }
                }
            }
        }
        #endregion

        #region Context Menus
        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
			menu.AddSeparator("");
            if (settings.currentGraph != null)
            {
                menu.AddItem(new GUIContent("Add State"), false, ContextCallback, UserActions.addState);
				menu.AddItem(new GUIContent("Add Portal"), false, ContextCallback, UserActions.makePortal);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Reset Panning"), false, ContextCallback, UserActions.resetPan);
			}
            else
            {
                menu.AddDisabledItem(new GUIContent("Add State"));
                menu.AddDisabledItem(new GUIContent("Add Comment"));
            }
            menu.ShowAsContext();
            e.Use();
        }

        void ModifyNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
			menu.AddSeparator("");
			if (selectedNode.drawNode is StateNode)
            {
                if (selectedNode.stateRef.currentState != null)
                {
                    menu.AddItem(new GUIContent("Add Condition"), false, ContextCallback, UserActions.addTransitionNode);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Add Condition"));
                }
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }
			else if (selectedNode.drawNode is PortalNode)
			{
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
			}
			else if (selectedNode.drawNode is TransitionNode)
            {
                if (selectedNode.isDuplicate || !selectedNode.isAssigned)
                {
                    menu.AddDisabledItem(new GUIContent("Make Transition"));
                }
                else
                {
                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, UserActions.makeTransition);
                }
                menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
            }
            menu.ShowAsContext();
            e.Use();
        }
        
        void ContextCallback(object o)
        {
            UserActions a = (UserActions)o;
            switch (a)
            {
                case UserActions.addState:
                    settings.AddNodeOnGraph(settings.stateNode, 300, 200, "State", mousePosition);                
                    break;
				case UserActions.makePortal:
					settings.AddNodeOnGraph(settings.portalNode, 100, 80, "Portal", mousePosition);
					break;
                case UserActions.addTransitionNode:
					AddTransitionNode(selectedNode, mousePosition);
					break;           
                default:
                    break;
                case UserActions.deleteNode:
					if (selectedNode.drawNode is TransitionNode)
					{
						Node enterNode = settings.currentGraph.GetNodeWithIndex(selectedNode.enterNode);
						if (enterNode != null)
							enterNode.stateRef.currentState.RemoveTransition(selectedNode.transRef.transitionId);
					}

					nodesToDelete++;
                    settings.currentGraph.DeleteNode(selectedNode.id);
                    break;
                case UserActions.makeTransition:
                    transitFromId = selectedNode.id;
                    settings.makeTransition = true;
                    break;
				case UserActions.resetPan:
					ResetScroll();
					break;
            }
			forceSetDirty = true;
		}

		public static Node AddTransitionNode(Node enterNode, Vector3 pos)
		{
			Node transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 50, "Condition", pos);
			transNode.enterNode = enterNode.id;
			Transition t = settings.stateNode.AddTransition(enterNode);
			transNode.transRef.transitionId = t.id;
			return transNode;
		}

		public static Node AddTransitionNodeFromTransition(Transition transition, Node enterNode, Vector3 pos)
		{
			Node transNode = settings.AddNodeOnGraph(settings.transitionNode, 200, 100, "Condition", pos);
			transNode.enterNode = enterNode.id;
			transNode.transRef.transitionId = transition.id;
			return transNode;
		}
		#endregion

		#region Helper Methods
		public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
			Vector3 startPos = Vector3.zero;
			Color c = Handles.color;
			Handles.Label(start.position, "0");
			if(start.center.x >= end.center.x) // RIGHT
			{
				if(start.center.y >= end.center.y) // RIGHT
					startPos = new Vector3(start.position.x, start.position.y, 0f);
				else
					startPos = new Vector3(start.position.x, start.position.y + start.size.y, 0f);
			}
			else // LEFT
			{
				if(start.center.y >= end.center.y) // RIGHT
					startPos = new Vector3(start.position.x + start.size.x, start.position.y, 0f);
				else
					startPos = new Vector3(start.position.x + start.size.x, start.position.y + start.size.y, 0f);
			}

			Vector3 endPos = Vector3.zero;
			if(end.center.x >= start.center.x) // RIGHT
			{
				if(end.center.y >= start.center.y) // RIGHT
				{
					c = Color.blue;
					endPos = new Vector3(end.position.x + end.size.x/2, end.position.y, 0f);
				}
				else
				{
					c = Color.green;
					endPos = new Vector3(end.position.x + end.size.x/2, end.position.y + end.size.y, 0f);
				}
			}
			else // LEFT
			{
				if(end.center.y >= start.center.y) // RIGHT
				{
					c = Color.red;
					endPos = new Vector3(end.position.x + end.size.x*1.5f, end.position.y, 0f);
				}
				else
				{
					c = Color.black;
					endPos = new Vector3(end.position.x + end.size.x*1.5f, end.position.y + end.size.y, 0f);
				}
			}

			Handles.color = c;

            //Vector3 endPos = new Vector3(end.x + (end.width * .5f), end.y + (end.height * .5f), 0);
            //Vector3 startTan = startPos + Vector3.right * 50;
            //Vector3 endTan = endPos + Vector3.left * 50;

			/*
            Color shadow = new Color(0, 0, 0, 1);
            for (int i = 0; i < 1; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, 4);

            }
			*/
			Vector3 dir = (startPos - endPos).normalized;
			Vector3 right = Quaternion.AngleAxis(-90, Vector3.forward) * dir;

			Handles.DrawAAConvexPolygon(new Vector3[] {startPos - right * 2, startPos + right * 2, endPos - right * 2, endPos + right * 2} );

			DrawTriangle(endPos, -dir, 10, c, true);

			//Handles.DrawLine(startPos, endPos);
			//Handles.DrawCube(0, endPos, Quaternion.identity, 10f);
            //Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 10);
        }

		public static void DrawTriangle(Vector3 position, Vector3 direction, float size, Color color, bool pivotOnPointyEdge)
		{
			Handles.color = color;
			Vector3 right = Quaternion.AngleAxis(-90, Vector3.forward) * direction;
			if(!pivotOnPointyEdge)
			{
				Handles.DrawAAConvexPolygon(
					new Vector3[]{
						position - right * size,
						position + right * size,
						position + direction * size * 2
					} 
				);

			}
			else
			{
				Handles.DrawAAConvexPolygon(
					new Vector3[]{
						position,
						position - (direction * size * 2) + right * size,
						position - (direction * size * 2) - right * size,
					}
				);
			}
		}

        public static void ClearWindowsFromList(List<Node>l)
        {
            for (int i = 0; i < l.Count; i++)
            {
          //      if (windows.Contains(l[i]))
            //        windows.Remove(l[i]);
            }
        }
        
        #endregion
    }
}
