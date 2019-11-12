using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Controller
{
    public Thrower[] throwers;
    public Animation animation;

    public override void OnPossess()
    {
        base.OnPossess();

        foreach(Thrower t in throwers)
        {
            t.Reload();
        }
    }

    internal override void OnLegsRetracted()
    {}

    internal override void OnLegsExtended()
    {}

    internal override void OnJump()
    {
        foreach(Thrower t in throwers)
        {
            t.Shoot();
            t.Reload();
        }
        if(animation != null) animation.Play();
    }
}
