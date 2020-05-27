using UnityEngine;

public class Shooter : Controller
{
    [Header("Shooter")]
    public Thrower[] throwers;
    public Feeder feeder;

    [Header("Movement")]
    public float rotateSpeed = 100f;
    public float lateralSpeed = 1f;
    public float stepShakeIntensity;
    public float stepShakeDuration;
    [Header("Shoot")]
    public float chargeMaxTime = 3f;
    public float chargeMaxForce = 1000f;
    public float aimDistanceMax = 10f;
    public float shootShakeIntensity;
    public float shootShakeDuration;

    [Header("Camera")]
    public float sensitivity = 1f;
    public float lerpRotation = 1f;
    public float lerpPosition = 1f;
    public float lerpFOV = 1f;
    public float aimFOV = 50f;
    public float defaultFOV = 70f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public Vector3 camerOffset;

    private Vector3 look;
    private float vertical;
    private float fov;
    private float chargeForce = 0f;
    private float chargeTime = 3f;
    public Vector3 aimOffset;

    [HideInInspector] public bool shoot;
    [HideInInspector] public bool aim;
    [HideInInspector] public float chargePercentage;

    public override void OnPossess()
    {
        base.OnPossess();
        foreach(Thrower t in throwers) t.Reload();
        OnAimUp();
        locomotion.locomotionAnimation.HeavyWalkCycle();
        locomotion.locomotionAnimation.legs.onStep += () =>
        {
            Shake(stepShakeIntensity, stepShakeDuration);
        };
    }

    private float shakeTimer;
    private float shakeDuration;
    private float shakeIntensity;
    private bool shaking = false;

    public void Shake(float intensity = 0.5f, float duration = 0.25f)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = shakeDuration;
        shaking = true;
    }

    internal override void Update()
    {
        base.Update();
        if (IsPossessed() && !AreLegsRetracted())
        {
            Vector3 camOffset = Vector3.zero;

            if (shaking)
            {
                if (shakeTimer > 0f) shakeTimer -= Time.deltaTime;
                else shaking = false;
                camOffset = Random.insideUnitCircle * shakeIntensity * (shakeTimer / shakeDuration);
            }

            // Calculate Camera Position
            Vector3 o = transform.position + camOffset;
            o += camerOffset.x * transform.right;
            o += camerOffset.y * transform.up;
            o += camerOffset.z * transform.forward;
            // Calculate Camera Rotation
            vertical -= Game.i.player.mapping.Axis(EAction.CAMERA_VERTICAL) * sensitivity;
            look = transform.eulerAngles;
            look.z = 0f;
            look.x = Mathf.Clamp(vertical, minAngle, maxAngle);
            // Apply Camera Position and Rotation
            Game.i.aperture.currentCamera.transform.rotation = Quaternion.Lerp(Game.i.aperture.currentCamera.transform.rotation, Quaternion.Euler(look), Time.deltaTime * lerpRotation);
            Game.i.aperture.currentCamera.transform.position = Vector3.Lerp(Game.i.aperture.currentCamera.transform.position, o, Time.deltaTime * lerpPosition);
            Game.i.aperture.currentCamera.fieldOfView = Mathf.Lerp(Game.i.aperture.currentCamera.fieldOfView, fov, Time.deltaTime * lerpFOV);

            float r = Game.i.player.mapping.Axis(EAction.MOVE_RIGHT_LEFT);
            float f = Game.i.player.mapping.Axis(EAction.MOVE_FORWARD_BACK);

            transform.Rotate(transform.up * Game.i.player.mapping.Axis(EAction.CAMERA_HORIZONTAL) * Time.deltaTime * rotateSpeed);
            if (IsGrounded()) rigidbody.AddForce(transform.right * r * Time.deltaTime * lateralSpeed);

            locomotion.locomotionAnimation.legs.transform.localEulerAngles = new Vector3(0f, r * 90f, 0f);
        }
    }

    public override void OnEject()
    {
        base.OnEject();
        Game.i.aperture.Unfreeze();
    }

    internal override void OnAimDown()
    {
        aim = true;
        fov = aimFOV;
        SoundPlayer.Play("sfx_click");
    }

    internal override void OnHoldShoot()
    {
        base.OnHoldShoot();
        if (!AreLegsRetracted())
        {
            if (chargeTime < chargeMaxTime) chargeTime += Time.deltaTime;
            else Shoot();

            chargePercentage = chargeTime / chargeMaxTime;
            chargeForce = chargePercentage * chargeMaxForce;
        }
    }

    internal override void OnShootDown()
    {
        base.OnShootDown();
        shoot = false;
    }

    public void Shoot()
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
                if(other.transform.TryGetComponent<NonPlayableCharacter>(out NonPlayableCharacter npc))
                {
                    npc.Stun(3f);
                }
            };

            t.Shoot();
            t.Reload();
        }

        chargePercentage = 0f;
        chargeForce = 0f;
        chargeTime = 0f;
        shoot = true;

        Shake(shootShakeIntensity, shootShakeDuration);
    }

    internal override void OnShootUp()
    {
        base.OnShootUp();
        if (!shoot) Shoot();
    }

    internal override void OnAimUp()
    {
        aim = false;
        fov = defaultFOV;
        SoundPlayer.Play("sfx_clack");
    }

    public override void Move(float fb, float rl)
    {
        if (IsFrozen()) return;
        BaseMove(Vector3.ClampMagnitude(new Vector3(0f, 0f, fb), 1f));
        locomotion.locomotionAnimation.moveInput = new Vector2(rl, fb);
    }

    internal override void OnLegsRetracted()
    {
        if(IsPossessed())
        {
            Game.i.aperture.Unfreeze();
            if (feeder != null) feeder.activated = true;
        }
    }

    internal override void OnLegsExtended()
    {
        if (IsPossessed())
        {
            Game.i.aperture.Freeze();
            if (feeder != null) feeder.activated = false;
        }
    }
}
