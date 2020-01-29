using UnityEngine;
namespace Behavior.NPC
{

	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/ReassureCharacter")]
	public class ReassureCharacter : AIAction
	{
		public override void Execute(NonPlayableCharacter target)
		{
			TheReef reef = target as TheReef;
			if (reef == null) return;
			reef.Reassure(reef.character);
		}
	}
}
