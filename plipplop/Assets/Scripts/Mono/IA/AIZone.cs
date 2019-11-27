using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZone
{
    List<Activity> activities = new List<Activity>();
    List<Feeder> feeders = new List<Feeder>();
    List<AIPath> paths = new List<AIPath>();

    public AIZone(Activity[] activities, Feeder[] feeders)
    {
        this.activities = new List<Activity>(activities);
        this.feeders = new List<Feeder>(feeders);
    }

    public AIZone()
    {

    }

    public void Register(Activity activity)
    {
        activities.AddUnique(activity);
    }

    public void Register(Feeder feeder)
    {
        feeders.AddUnique(feeder);
    }

    public void Register(AIPath path)
    {
        paths.Add(path);
    }

    public AIPath GetRandomPath()
    {
        return paths.PickRandom();
    }

    public Feeder[] GetFeeders()
    {
        return feeders.ToArray();
    }

    public AIPath[] GetPaths()
    {
        return paths.ToArray();
    }
    
}
