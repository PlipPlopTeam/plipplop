using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feeder : MonoBehaviour
{
    [Header("Settings")]
    public Transform givePoint;
    public FoodData food;
    public int stock;
    public float timeToServe = 1f;
    public float distanceBetween = 1.5f;

    List<NonPlayableCharacter> clients = new List<NonPlayableCharacter>();
    NonPlayableCharacter next;
    bool serving = false;
    float serveTimer = 0f;

    public void Enter(NonPlayableCharacter client)
    {
        clients.Add(client);
        client.agentMovement.GoThere(transform.position + transform.forward * clients.Count * distanceBetween);
        if(!serving) Next();
    }

    public void Exit(NonPlayableCharacter client)
    {
        // Get the f*** out now
        client.agentMovement.GoThere(
            new Vector3(
                transform.position.x + Random.Range(-10f, 10f),
                transform.position.y, 
                transform.position.z + Random.Range(-10f, 10f)
            )
        );

        clients.Remove(client);
    }

    public void Serve(NonPlayableCharacter client)
    {
        Vector3 pos = givePoint == null ? transform.position : givePoint.position;

        Food fo = Instantiate(food.visual, pos, Quaternion.identity).AddComponent<Food>();;
        fo.data = food;
        client.food = fo;
        client.Carry(fo);

        Exit(client);
        serving = false;
        Next();
    }

    public void Next()
    {
        if(clients.Count > 0)
        {
            for(int i = 0; i < clients.Count; i++)
            {
                clients[i].agentMovement.GoThere(transform.position + transform.forward * (i + 1) * distanceBetween);
            }

            next = clients[0];
            serveTimer = timeToServe;
            serving = true;
        }
    }

    public void Update()
    {
        if(serving)
        {
            if(serveTimer > 0f) serveTimer -= Time.deltaTime;
            else
            {
                if(next != null) Serve(next);
            }
        }
    }
}
