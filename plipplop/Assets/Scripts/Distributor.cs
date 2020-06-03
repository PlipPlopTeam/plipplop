using UnityEngine;

public class Distributor : Shooter
{
    [Header("Distributor")]
    public Thrower[] throwers;
    public Feeder feeder;
    public Animation anim;

    [Header("Shoot")]
    public float chargeMaxTime = 3f;
    public float chargeMaxForce = 1000f;
    [Range(0f, 1f)] public float chargeTreshold = 0.1f;
    public float aimDistanceMax = 10f;
    public float shootShakeIntensity;
    public float shootShakeDuration;
    public float shootCooldown = 1f;

    private float cooldown;
    private float chargeForce = 0f;
    private float chargeTime = 3f;

    [HideInInspector] public bool shoot;
    [HideInInspector] public bool aim;

    bool right = false;
    public override void OnPossess()
    {
        base.OnPossess();
        foreach (Thrower t in throwers) t.Reload();
        OnAimUp();
        locomotion.locomotionAnimation.HeavyWalkCycle();
        locomotion.locomotionAnimation.legs.onStep += () => Step();
        Load();
    }

    public void Step()
    {
        Shake(stepShakeIntensity, stepShakeDuration);
        if (right)
        {
            SoundPlayer.PlayAtPosition("sfx_plipstep_machine", transform.position);
            right = false;
        }
        else
        {
            SoundPlayer.PlayAtPosition("sfx_plopstep_machine", transform.position);
            right = false;
        }
    }

    internal override void Update()
    {
        base.Update();
        if (IsPossessed() && !AreLegsRetracted())
        {
            if (cooldown > 0f) cooldown -= Time.deltaTime;
        }
    }

    public bool Ready()
    {
        return cooldown <= 0f;
    }

    public override void OnEject()
    {
        base.OnEject();
        Game.i.aperture.Unfreeze();
    }

    internal override void OnAimDown()
    {
        if (!AreLegsRetracted())
        {
            aim = true;
            fov = aimFOV;
            SoundPlayer.Play("sfx_click");
        }
    }

    internal override void OnHoldShoot()
    {
        base.OnHoldShoot();
        if (!AreLegsRetracted() && Ready())
        {
            if (chargeTime < chargeMaxTime) chargeTime += Time.deltaTime;
            else if (chargeTime / chargeMaxTime > chargeTreshold) Shoot();

            chargePercentage = chargeTime / chargeMaxTime;
            chargeForce = chargePercentage * chargeMaxForce;
        }
    }

    internal override void OnShootDown()
    {
        base.OnShootDown();
        if (!AreLegsRetracted() && Ready())
        {
            shoot = false;
        }
    }

    public void Shoot()
    {
        if (!AreLegsRetracted())
        {
            Ray ray = Game.i.aperture.currentCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Vector3 target = transform.position + ray.direction * aimDistanceMax;

            if (Physics.Raycast(ray, out RaycastHit h, aimDistanceMax))
            {
                target = h.point;
            }

            foreach (Thrower t in throwers)
            {
                t.gunEnd.forward = (target - t.gunEnd.transform.position).normalized;
                t.force = chargeForce;

                t.arrow.onImpact += (other) =>
                {
                    if (other.transform.TryGetComponent<NonPlayableCharacter>(out NonPlayableCharacter npc))
                    {
                        npc.Stun(3f);
                    }
                    SoundPlayer.PlayAtPosition("sfx_can_bonk", other.GetContact(0).point);
                    t.arrow.rb.velocity /= 10f;
                };

                t.Shoot();
                t.Reload();
            }

            cooldown = shootCooldown;

            if (anim != null) anim.Play("A_Shoot");
            Load();
            Shake(shootShakeIntensity, shootShakeDuration);
        }
    }

    private void Load()
    {
        chargePercentage = 0f;
        chargeForce = 0f;
        chargeTime = 0f;
        shoot = true;
    }

    internal override void OnShootUp()
    {
        base.OnShootUp();
        if (!AreLegsRetracted() && !shoot) Shoot();
    }

    internal override void OnAimUp()
    {
        if (!AreLegsRetracted())
        {
            aim = false;
            fov = defaultFOV;
            SoundPlayer.Play("sfx_clack");
        }
    }

    internal override void OnLegsRetracted()
    {
        base.OnLegsRetracted();
        if (IsPossessed())
        {
            if (feeder != null) feeder.activated = true;
        }
    }

    internal override void OnLegsExtended()
    {
        base.OnLegsExtended();
        if (IsPossessed())
        {
            OnAimUp();
            if (feeder != null) feeder.activated = false;
        }
    }
}
