using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_DestroyableBuilding : MonoBehaviour
{
    public GameObject destroyable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DBG_Mecha>())
        {
            destroyable.transform.parent = null;
            destroyable.SetActive(true);
            Destroy(gameObject);
        }
    }
}
