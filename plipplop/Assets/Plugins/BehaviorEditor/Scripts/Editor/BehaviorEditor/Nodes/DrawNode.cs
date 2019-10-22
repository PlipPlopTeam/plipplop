using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PP.Behavior
{
    public abstract class DrawNode : ScriptableObject
    {
        public abstract void DrawWindow(Node b);
        public abstract void DrawCurve(Node b);
    }
}
