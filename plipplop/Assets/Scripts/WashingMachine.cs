using UnityEngine;

public class WashingMachine : Shooter
{
    [Header("Washing Machine")]
    public ParticleSystem ps;
    public Animation anim;

    public float stunDuration = 1f;
    public float radius = 1f;
    public float cooldown = 1f;
    public Vector3 offset;

    private float timer;

    internal override void OnShootDown()
    {
        base.OnShootDown();
        if (!AreLegsRetracted() && timer <= 0f) Shoot();
    }

    internal override void Update()
    {
        base.Update();

        if (timer > 0f) timer -= Time.deltaTime;
    }

    void Shoot()
    {
        if(ps != null) ps.Play();
        if (anim != null) anim.Play("A_Shoot");

        RaycastHit[] hits;
        Vector3 pos = transform.position + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
        hits = Physics.SphereCastAll(pos, radius, transform.forward);

        foreach(RaycastHit h in hits)
        {
            if(h.transform.gameObject.TryGetComponent<NonPlayableCharacter>(out NonPlayableCharacter npc))
            {
                npc.Stun(stunDuration);
            }
        }
    }
}
