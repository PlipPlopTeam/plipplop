using UnityEngine;
using PP;

namespace NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/GetNearestFeeder")]
    public class GetNearestFeeder : Action
    {
		public override void Execute(StateManager state)
		{
			NonPlayableCharacter npc = (NonPlayableCharacter)state;
			if(npc != null && Zone.i != null)
            {
                if(Zone.i.feeders.Count > 0)
                {
                    Feeder feeder = null;
                    foreach(Feeder f in Zone.i.feeders)
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
