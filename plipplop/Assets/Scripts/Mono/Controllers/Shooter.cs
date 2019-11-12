using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Controller
{
    public Thrower[] throwers;
    public Animation animation;

    internal override void OnLegsRetracted()
    {}

    internal override void OnLegsExtended()
    {}

    internal override void OnJump()
    {
        foreach(Thrower t in throwers)
        {
            t.Reload();
            t.Shoot();
        }
        animation.Play();
    }
}
