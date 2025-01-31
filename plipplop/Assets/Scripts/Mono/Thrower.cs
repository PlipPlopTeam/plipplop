﻿using UnityEngine;

public class Thrower : MonoBehaviour
{
    [Header("Referencies")]
    public GameObject prefabToThrow;
    public Transform gunEnd;
    public ParticleSystem ps;
    public string sfx;
    public bool show;

    [Header("Settings")]
    public float force = 1000f;
    public float torque = 0f;

    [Header("Settings")]
    public Projectile arrow;

    public void Reload()
    {
        if(arrow == null && prefabToThrow != null)
        {
            arrow = Instantiate(prefabToThrow, gunEnd.position, gunEnd.rotation)
            .GetComponent<Projectile>();
            arrow.Stuck();
            arrow.transform.SetParent(transform);
            arrow.gameObject.SetActive(show);
        }
    }

    public void Shoot()
    {
        if(arrow != null)
        {
            arrow.gameObject.SetActive(true);
            arrow.transform.SetParent(null);
            arrow.Throw(gunEnd.forward, force, torque);
            arrow = null;
        }
        if(ps != null) ps.Play();
        if (sfx != "") SoundPlayer.PlayAtPosition(sfx, transform.position);
    }

    public void OnDestroy()
    {
        if(arrow != null) Destroy(arrow.gameObject);
    }
}
