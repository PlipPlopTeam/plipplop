using UnityEngine;

namespace Behavior.NPC
{
    [CreateAssetMenu(menuName = "Behavior/Action/NonPlayableCharacter/PlaySound")]
    public class PlaySound : AIAction
    {
        public string sname = "";
        public float volume = 1f;
        public float pitch = 1f;
        public bool playAtFeet = true;
        public bool fadeIn = false;
        public override void Execute(NonPlayableCharacter target)
        {
            NonPlayableCharacter npc = target;
            if (npc != null && sname != "")
            {
                if(playAtFeet)
                    SoundPlayer.PlayAtPosition(sname, npc.transform.position, volume, false, fadeIn);
                else
                    SoundPlayer.Play(sname, volume, pitch, fadeIn);
            }
        }
    }
}