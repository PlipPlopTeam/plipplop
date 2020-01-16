using UnityEngine;

namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/Kick Carried Possessed Controller")]
	public class KickCarriedPossessedController : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null)
			{
				if (npc.carried != null && npc.carried.Self().gameObject.TryGetComponent(out Controller result))
				{
					npc.Kick(result);
				}
			}
		}
	}
}
