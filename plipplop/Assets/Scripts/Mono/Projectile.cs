using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Referencies")]
    public Collider cd;
    public Rigidbody rb;

    [Header("Settings")]
    public bool stuckOnAwake = true;
    public bool stuckOnImpact = true;
    public bool piercing = false;
    public bool hitTriggers = false;
    public float destroyAfter = 0f;
    
    [Header("READ ONLY")]
    public bool stuck;
    public float timer;
    public bool beingDestroy;

    void Start()
    {
        if(stuckOnAwake) Stuck();
    }

    void Update()
    {
        if(beingDestroy)
        {
            if(timer > 0) timer -= Time.deltaTime;
            else Destroy(gameObject);
        }
    }

    public virtual void Stuck(Transform on = null)
    {
        cd.isTrigger = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        if(on)
        {
            transform.SetParent(on);
            transform.forward = -(transform.position - on.position).normalized;
            //transform.localScale = new Vector3(1f, 1f, 1f);
        }

        beingDestroy = false;
        stuck = true;
    }

    public virtual void Unstuck()
    {
        cd.isTrigger = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        transform.SetParent(null);

        stuck = false;
    }

    public virtual void Throw(Vector3 direction, float force)
    {
        Unstuck();
        rb.AddForce(direction * force * Time.deltaTime, ForceMode.Impulse);
        stuck = false;

        if(destroyAfter > 0f)
        {
            beingDestroy = true;
            timer = destroyAfter;
        }
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        if(stuckOnImpact) Stuck(other.transform);
    }
}
