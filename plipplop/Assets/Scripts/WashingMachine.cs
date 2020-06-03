using UnityEngine;

public class WashingMachine : Shooter
{
    [Header("Washing Machine")]
    public ParticleSystem ps;
    public Animation anim;

    internal override void OnShootDown()
    {
        base.OnShootDown();
        if (!AreLegsRetracted()) Shoot();
    }

    void Shoot()
    {
        if(ps != null) ps.Play();
        if (anim != null) anim.Play("A_Shoot");
    }
}
