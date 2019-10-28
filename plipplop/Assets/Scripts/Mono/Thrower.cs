using UnityEngine;

public class Thrower : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject prefabToThrow;
    public Transform gunEnd;

    [Header("Settings")]
    public float force;

    private Projectile arrow;

    public void Reload()
    {
        if(arrow == null)
        {
            arrow = Instantiate(prefabToThrow, gunEnd.position, gunEnd.rotation)
            .GetComponent<Projectile>();
            arrow.Stuck();
        }
    }

    public void Shoot()
    {
        if(arrow != null)
        {
            arrow.Throw(gunEnd.forward, force);
            arrow = null;
        }
    }

    public void OnDestroy()
    {
        if(arrow != null) Destroy(arrow.gameObject);
    }
}
