using UnityEngine;
namespace Behavior.NPC
{

	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/SearchCharacterInState")]
	public class SearchCharacterInState : AIAction
	{
		public string stateName = "";
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc == null || stateName == "") return;
			NonPlayableCharacter[] characters = npc.sight.Scan<NonPlayableCharacter>();
			foreach (NonPlayableCharacter c in characters)
			{
				
				if (c.graph.GetState().name == stateName)
				{
					npc.character = c;
					return;
				}
			}
		}
	}
}