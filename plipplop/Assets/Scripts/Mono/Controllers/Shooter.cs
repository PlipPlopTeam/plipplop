using UnityEngine;

public class Shooter : Controller
{
    [Header("Shooter")]
    public Thrower[] throwers;

    [Header("Movement")]
    public float rotateSpeed = 100f;
    public float lateralSpeed = 1f;

    [Header("Shoot")]
    public float chargeMaxTime = 3f;
    public float chargeMaxForce = 1000f;
    public float aimDistanceMax = 10f;

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
            t.Shoot();
            t.Reload();
        }

        chargePercentage = 0f;
        chargeForce = 0f;
        chargeTime = 0f;
        shoot = true;
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

    internal override void Update()
    {
        base.Update();
        if (IsPossessed() && !AreLegsRetracted())
        {
            // Calculate Camera Position
            Vector3 o = transform.position;
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

            transform.Rotate(transform.up * Game.i.player.mapping.Axis(EAction.CAMERA_HORIZONTAL) * Time.deltaTime * rotateSpeed);
            transform.position += transform.right * Game.i.player.mapping.Axis(EAction.MOVE_RIGHT_LEFT) * Time.deltaTime * lateralSpeed;
        }
    }

    public override void Move(float fb, float rl)
    {
        if (IsFrozen()) return;
        BaseMove(Vector3.ClampMagnitude(new Vector3(0f, 0f, fb), 1f));
    }

    internal override void OnLegsRetracted()
    {
        Game.i.aperture.Unfreeze();
    }

    internal override void OnLegsExtended()
    {
        Game.i.aperture.Freeze();
    }
}
