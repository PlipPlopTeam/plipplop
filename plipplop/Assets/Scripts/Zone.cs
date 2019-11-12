using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public static Zone i;
    public List<Activity> activities = new List<Activity>();
    public List<NonPlayableCharacter> actors = new List<NonPlayableCharacter>();
    
    public AgentMovement.Path[] paths;

    void Awake()
    {
        if(Zone.i != null) Destroy(Zone.i.gameObject);
        Zone.i = this;

        foreach(Activity a in FindObjectsOfType<Activity>())
            activities.Add(a);

        foreach(NonPlayableCharacter npc in FindObjectsOfType<NonPlayableCharacter>())
            actors.Add(npc);
    }
    
    private void OnDrawGizmosSelected() 
    {
        foreach(AgentMovement.Path path in paths)
        {
            foreach(Vector3 point in path.points)
            {
                Gizmos.DrawWireMesh((Mesh)Resources.Load("Meshes/WireFlag", typeof(Mesh)), point, Quaternion.identity, new Vector3(1f, 1f, 1f));
            }
        }
    }

    public AgentMovement.Path GetRandomPath()
    {
        return paths[Random.Range(0, paths.Length)];
    }
}
