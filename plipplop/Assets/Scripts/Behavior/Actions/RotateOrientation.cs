using UnityEngine;
namespace Behavior.NPC
{
	[CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/RotateOrientation")]
	public class RotateOrientation : AIAction
	{
		[System.Serializable]
		public class OrientationCall
		{
			public float angle;
			public float time;
		}

		public OrientationCall[] orientations;
		public override void Execute(NonPlayableCharacter target)
		{
			NonPlayableCharacter npc = target;
			if (npc != null)
			{
				if(orientations.Length > 0)
				{
					float t = 1f;
					foreach (OrientationCall oc in orientations)
					{
						npc.WaitAndDo(oc.time, () =>
						{
							npc.agentMovement.Orient(new Vector3(Mathf.Cos(oc.angle), 0f, Mathf.Sin(oc.angle)));
						});
						 t += oc.time;
					}
					npc.Wait(t);
				}
				else
				{
					npc.Wait(2f);
				}
			}
		}
	}
}