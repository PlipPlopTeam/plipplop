using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_Move : MonoBehaviour
{
    public Vector3 direction;


    // Update is called once per frame
    void Update()
    {
        transform.position += (direction * Time.deltaTime);
    }
}
