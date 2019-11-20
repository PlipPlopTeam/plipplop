using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : Feeder
{
    [Header("Dispenser")]
    public float distanceBetween = 1.5f;
    public float timeToServe = 1f;

    List<NonPlayableCharacter> clients = new List<NonPlayableCharacter>();
    NonPlayableCharacter next;
    bool serving = false;
    float serveTimer = 0f;

    public override void Catch(NonPlayableCharacter client)
    {
        base.Catch(client);

        clients.Add(client);
        client.agentMovement.GoThere(transform.position + transform.forward * clients.Count * distanceBetween);
        if(!serving) Next();
    }

    public override void Serve(NonPlayableCharacter client)
    {
        base.Serve(client);

        clients.Remove(client);
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
            else if(next != null) Serve(next);
        }
    }
}
