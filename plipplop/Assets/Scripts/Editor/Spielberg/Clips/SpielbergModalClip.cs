using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spielberg.Clips
{
    public class SpielbergModalClip<TBehaviour> : SpielbergClip where TBehaviour : SpielbergClipBehaviour, new()
    {
        public TBehaviour behaviour = new TBehaviour();

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
