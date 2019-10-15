using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public LocomotionPreset preset;
    public float legsHeight = 1f;
    public float jump = 10f;
    public float groundCheckRange = 1f;

    public Vector3 legsOffset;

    [HideInInspector] new public Rigidbody rigidbody;
    [HideInInspector] public Vector3 targetDirection;

    float currentSpeed = 0f;
    CapsuleCollider legsCollider;
    Legs legs;

    private void Awake()
    {
        legsCollider = gameObject.AddComponent<CapsuleCollider>();
        preset = preset ? preset : Game.i.defaultLocomotion;
        rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        legsCollider.height = legsHeight;
        legsCollider.center = legsOffset + new Vector3(0f, -legsHeight / 2, 0f);

    }

    public bool AreLegsRetracted()
    {
        return legs == null || !legs.gameObject.activeSelf;
    }

    private void GrowLegs()
    {
        legs = Instantiate(Game.i.library.legsPrefab, transform)
        .GetComponent<Legs>();
        legs.body = transform;
        legs.transform.localPosition = legsOffset;
        foreach (Leg l in legs.legs) l.maxFootDistance = legsHeight + 1f;
    }

    public void RetractLegs()
    {
        if (!legs) GrowLegs();

        legs.gameObject.SetActive(false);
        legsCollider.enabled = false;
    }
    public void ExtendLegs()
    {
        if (!legs) GrowLegs();
        legs.gameObject.SetActive(true);
        legsCollider.enabled = true;
        rigidbody.drag = preset.baseDrag;

        Vector3 surfacePosition = GetBelowSurface();
        if (surfacePosition != Vector3.zero) {
            transform.position = new Vector3(transform.position.x, surfacePosition.y + legsHeight, transform.position.z);
        }
    }

    public void Move(Vector3 direction)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + preset.acceleration * Time.deltaTime, 0f, preset.speed * direction.magnitude);

        Vector3 clampDirection = Vector3.ClampMagnitude(direction, 1f);
        //Vector3 camdir = Vector3.one;//new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

        //Vector3 dir = new Vector3(clampDirection.x * Game.i.aperture.Right().x,  0f, clampDirection.z  * Game.i.aperture.Right().z);
        Vector3 dir = clampDirection.x * Game.i.aperture.Right() + clampDirection.z * Game.i.aperture.Forward();
        // Add Movement Force
        rigidbody.AddForce(dir * Time.deltaTime * currentSpeed);

        // Rotate legs
        if (dir != Vector3.zero) targetDirection = dir;

        transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * 4f);

        legs.velocity = rigidbody.velocity;
    }

    public void Fall()
    {
        if (rigidbody != null && !IsGrounded() && !AreLegsRetracted()) {
            Vector3 v = rigidbody.velocity + Vector3.down * preset.strength;
            if (v.y < -preset.maxFallSpeed) v.y = -preset.maxFallSpeed;
            rigidbody.velocity = v;
        }
    }

    private Vector3 GetBelowSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + legsOffset + new Vector3(0f, -legsHeight, 0f), Vector3.down, out hit)) return hit.point;
        return Vector3.zero;
    }

    public bool IsGrounded()
    {
        return
            AreLegsRetracted() ?
                Physics.Raycast(transform.position + legsOffset, -transform.up, groundCheckRange) :
                // Magic 0.1f so the raycast can start above ground and not inside ground
                Physics.Raycast(transform.position + legsOffset - new Vector3(0f, legsHeight - 0.25f, 0f), -transform.up, groundCheckRange);
    }


    // Draw a gizmo if i'm being possessed
    void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) {
            // Legs
            Gizmos.color = Color.red;
            if (AreLegsRetracted())
                Gizmos.DrawLine(transform.position + legsOffset, transform.position + legsOffset + (-transform.up * groundCheckRange));
            else
                Gizmos.DrawLine(transform.position + legsOffset - new Vector3(0f, legsHeight, 0f), transform.position + legsOffset - new Vector3(0f, legsHeight, 0f) + (-transform.up * groundCheckRange));

        }
    }
}
