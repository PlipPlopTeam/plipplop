using UnityEngine;

public class Thrower : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject prefabToThrow;
    public Transform gunEnd;
    public ParticleSystem ps;

    [Header("Settings")]
    public float force;
    private Projectile arrow;

    public void Reload()
    {
        if(arrow == null && prefabToThrow != null)
        {
            arrow = Instantiate(prefabToThrow, gunEnd.position, gunEnd.rotation)
            .GetComponent<Projectile>();
            arrow.Stuck();
            arrow.transform.SetParent(transform);
        }
    }

    public void Shoot()
    {
        if(arrow != null)
        {
            arrow.transform.SetParent(null);
            arrow.Throw(gunEnd.forward, force);
            arrow = null;
        }

        if(ps != null) ps.Play();
    }

    public void OnDestroy()
    {
        if(arrow != null) Destroy(arrow.gameObject);
    }
}
