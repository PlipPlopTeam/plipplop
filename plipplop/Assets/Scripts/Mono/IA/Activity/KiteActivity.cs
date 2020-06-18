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
        
        npc.movement.OrientToward(kite.visuals.transform.position);

    }

    public override void StopSpectate(NonPlayableCharacter npc)
    {
        base.StopSpectate(npc);
        
        npc.look.LooseFocus();
    }

    public override void StartUsing(NonPlayableCharacter user)
    {
        base.StartUsing(user);
        user.movement.Stop();
		user.animator.SetBool("Holding", true);

		if (kite != null)
		{
			user.look.FocusOn(kite.visuals.transform);
			user.movement.OrientToward(kite.visuals.transform.position);
		}
        user.skeleton.Attach(transform, Cloth.ESlot.RIGHT_HAND);

        kite.StartFly();
        full = true;
    }

    public override void Update()
    {
        base.Update();

        if (users.Count > 0)
        {
            users[0].movement.OrientToward(kite.visuals.transform.position);
        }

        foreach (var _s  in spectators)
        {
            _s.movement.OrientToward(kite.visuals.transform.position);
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
