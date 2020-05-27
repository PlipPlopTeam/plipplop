using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cake : Feeder
{
    [Header("Cake")]
    public List<GameObject> pieces = new List<GameObject>();

    public override void Catch(NonPlayableCharacter npc)
    {
        base.Catch(npc);
        npc.movement.GoThere(transform.position, true);
        npc.movement.onDestinationReached += () =>
        {
            Serve(npc);
        };
    }

    public override void Serve(NonPlayableCharacter npc)
    {
        base.Serve(npc);

        if(pieces.Count > 0)
        {
            int rand = Random.Range(0, pieces.Count);
            Destroy(pieces[rand]);
            pieces.RemoveAt(rand);
        }
    }
}
