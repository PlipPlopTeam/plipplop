using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZone
{
    List<Activity> activities = new List<Activity>();
    List<Feeder> feeders = new List<Feeder>();
    List<AIPath> paths = new List<AIPath>();
    List<Container> containers = new List<Container>();
    List<BirdArea> birdAreas = new List<BirdArea>();
	List<BirdPath> birdPaths = new List<BirdPath>();

	public AIZone(Activity[] activities, Feeder[] feeders)
    {
        this.activities = new List<Activity>(activities);
        this.feeders = new List<Feeder>(feeders);
    } 

    public AIZone()
    {

    }

    public Container GetContainerMadeFor(Item.EType type)
    {
        foreach(Container c in containers)
        {
            if (c.madeFor.Contains(type)) return c;
        }
        return null;
    }

    public Container GetNearestContainerMadeFor(Vector3 position, Item.EType type)
    {
        Container result = null;
        foreach (Container c in containers)
        {
            if (c.madeFor.Contains(type))
            {
                if(result != null)
                {
                    if (Vector3.Distance(c.transform.position, position) < Vector3.Distance(result.transform.position, position))
                    {
                        result = c;
                    }
                }
            }
        }
        return result;
    }

    public void Register(Container c)
    {
        containers.Add(c);
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

	public void Register(BirdPath path)
	{
		birdPaths.Add(path);
	}

	public void Register(BirdArea area)
	{
		birdAreas.Add(area);
	}

	public AIPath GetRandomPath()
    {
        return paths.PickRandom();
    }

    public BirdPath GetRandomBirdPath()
    {
        if (birdAreas.Count > 0) return birdPaths.PickRandom();
        else return null;
    }

    public BirdArea GetRandomBirdArea()
    {
        if (birdAreas.Count > 0) return birdAreas.PickRandom();
        else return null;
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
