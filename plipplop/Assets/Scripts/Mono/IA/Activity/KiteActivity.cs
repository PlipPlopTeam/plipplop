using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiteActivity : Activity
{
    public Kite kite;


    public override void StartSpectate(NonPlayableCharacter npc)
    {
        base.StartSpectate(npc);
        
        npc.look.FocusOn(kite.visuals.transform);
        
        npc.agentMovement.OrientToward(kite.visuals.transform.position);

    }

    public override void StopSpectate(NonPlayableCharacter npc)
    {
        base.StopSpectate(npc);
        
        npc.look.LooseFocus();
    }

    public override void StartUsing(NonPlayableCharacter user)
    {
        base.StartUsing(user);
        
        user.agentMovement.Stop();
        
        user.look.FocusOn(kite.visuals.transform);

        user.agentMovement.OrientToward(kite.visuals.transform.position);
        
        user.skeleton.Attach(transform, Cloth.ESlot.RIGHT_HAND);
        
        user.animator.SetBool("Holding", true);
        kite.StartFly();

        full = true;
    }

    public override void Update()
    {
        base.Update();

        if (users.Count > 0)
        {
            users[0].agentMovement.OrientToward(kite.visuals.transform.position);
        }

        foreach (var _s  in spectators)
        {
            _s.agentMovement.OrientToward(kite.visuals.transform.position);
        }
    }

    public override void StopUsing(NonPlayableCharacter user)
    {
        base.StopUsing(user);
        
        user.look.LooseFocus();

        user.skeleton.Drop(Cloth.ESlot.RIGHT_HAND);
        
        user.animator.SetBool("Holding", false);
        
        kite.StopFly();
        
        full = false;

    }
}
