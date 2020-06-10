using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/PlayRandomSound")]
    public class PlayRandomSound : AIAction
    {
        public string[] sounds;
        public float volume = 1f;
        public float pitch = 1f;
        public bool playAtFeet = true;
        public bool fadeIn = false;
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && sounds.Length > 0)
            {
				string s = sounds[Random.Range(0, sounds.Length)];

                if(playAtFeet)
                    SoundPlayer.PlayAtPosition(s, npc.transform.position, volume, false, fadeIn);
                else
                    SoundPlayer.Play(s, volume, pitch, fadeIn);
            }
        }
    }
}