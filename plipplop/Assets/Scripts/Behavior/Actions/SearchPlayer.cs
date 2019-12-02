using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchPlayer")]
	public class SearchPlayer : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null)
			{
				Controller[] controllers = npc.sight.Scan<Controller>();
				foreach (Controller c in controllers)
				{
					if (c.IsPossessed())
					{
						npc.player = c;
						break;
					}
				}
			}
		}
	}
}

