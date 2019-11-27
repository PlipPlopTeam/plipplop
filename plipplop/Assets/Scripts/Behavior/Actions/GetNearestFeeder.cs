using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GetNearestFeeder")]
    public class GetNearestFeeder : AIAction
    {
		public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null)
            {
                var feeders = Game.i.aiZone.GetFeeders();
                if (feeders.Length > 0)
                {
                    Feeder feeder = null;
                    foreach(Feeder f in feeders)
                    {
                        if(feeder == null) 
                        {
                            if(f.stock > 0) feeder = f;
                        }
                        else
                        {
                            if(Vector3.Distance(feeder.transform.position, npc.transform.position) > Vector3.Distance(f.transform.position, npc.transform.position))
                            {
                                if(f.stock > 0) feeder = f;
                            }
                        }
                    }
                    npc.feeder = feeder;
                }
            }
		}
    }
}
