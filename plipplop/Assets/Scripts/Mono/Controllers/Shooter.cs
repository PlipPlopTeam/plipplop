using UnityEngine;

public class Shooter : Controller
{
    [Header("Shooter")]
    public Thrower[] throwers;
    public float rotateSpeed = 100f;

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
    [HideInInspector] public bool aim;

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

    internal override void OnJump()
    {
        foreach(Thrower t in throwers)
        {
            t.Shoot();
            t.Reload();
        }
    }
}
