using UnityEngine;

public class Shooter : Controller
{
    [Header("Movement")]
    public float rotateSpeed = 100f;
    public float lateralSpeed = 3000f;
    public float stepShakeIntensity = 0.5f;
    public float stepShakeDuration = 0.5f;

    [Header("Camera")]
    public float sensitivity = 0.5f;
    public float lerpRotation = 10f;
    public float lerpPosition = 5f;
    public float lerpFOV = 5f;
    public float aimFOV = 40f;
    public float defaultFOV = 70f;
    public float minAngle = -45f;
    public float maxAngle = 45f;
    public Vector3 cameraOffset;

    internal Vector3 look;
    internal float vertical;
    internal float fov;
    // internal Vector3 aimOffset; // Never used
    internal float shakeTimer;
    internal float shakeDuration;
    internal float shakeIntensity;
    internal bool shaking = false;
    [HideInInspector] public float chargePercentage;


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
            o += cameraOffset.x * transform.right;
            o += cameraOffset.y * transform.up;
            o += cameraOffset.z * transform.forward;
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

    public override void OnPossess()
    {
        base.OnPossess();
        fov = defaultFOV;

        InputDisplay _inputDisplay = FindObjectOfType<InputDisplay>();
        if (_inputDisplay != null)
        {
            _inputDisplay.ShowShooterInputs();
        }
    }

    public override void Move(float fb, float rl)
    {
        if (IsFrozen()) return;
        BaseMove(Vector3.ClampMagnitude(new Vector3(0f, 0f, fb), 1f));
        locomotion.locomotionAnimation.moveInput = new Vector2(rl, fb);
    }

    internal override void OnLegsRetracted()
    {
        if(IsPossessed()) Game.i.aperture.Unfreeze();
    }

    internal override void OnLegsExtended()
    {
        if (IsPossessed()) Game.i.aperture.Freeze();
    }
}
