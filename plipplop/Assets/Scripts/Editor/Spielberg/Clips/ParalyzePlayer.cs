using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class ParalyzePlayer : SpielbergClip
    {
        public SpielbergParalyzePlayerClipBehaviour behaviour = new SpielbergParalyzePlayerClipBehaviour();

        public override SpielbergClipBehaviour GetBehaviour()
        {
            return behaviour;
        }

        public override string GetDisplayName()
        {
            return behaviour.ToString();
        }
    }
}
