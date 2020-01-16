using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DestroyWhenShurikenDone : MonoBehaviour
{
    ParticleSystem shuriken;

    private void Awake()
    {
        shuriken = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!shuriken.IsAlive()) {
            Destroy(gameObject, Random.value);
        }
    }


}
