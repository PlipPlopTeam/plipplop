using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Activity
{
    [Header("BALLOON")]
    public float minDistanceBetween = 3f;
    public float maxDistanceBetween = 5f;
	public float distanceMax = 3f;
    public float timeBetweenThrows = 2f;
    public float verticalForce = 50000f;
    public float horizontalForce = 25000f;

	private Vector3 originPosition;
    private int carrier = 0;
    private float throwTimer;
    private List<bool> inPlace = new List<bool>();
    private bool playing;
    private bool flying;

	public override void Break()
	{
		base.Break();
		if(users.Count > 0) users[carrier].Drop();
		Initialize();
	}

	public override void StopUsing(NonPlayableCharacter user)
	{

		if (user.IsCarrying(this)) user.Drop();
		if (user.look != null) user.look.LooseFocus();

		user.StopCollecting();

		base.StopUsing(user);
		user.GetUp();

		if (users.Count > 1)
		{
			carrier = Next();
			users[carrier].Collect(this);
		}
		else Initialize();
	}

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		if (user.look != null) user.look.FocusOn(transform);
		user.movement.Stop();

		if (users.Count > 1) GetInPlace();
		else
		{
			users[carrier].Collect(this, () => 
			{
				/*
				if (users.Count <= 1)
				{
					users[carrier].Sit();
				}
				else
				{
					GetInPlace();
				}
				*/
				GetInPlace();
			});
		}
	}

	bool GoodPositions()
	{
		float distance = 0f;
		for(int i = 0; i < users.Count - 1; i++) distance += Vector3.Distance(users[i].transform.position, users[i + 1].transform.position);
		return distance < distanceMax * users.Count;
	}

    void GetInPlace()
    {
		playing = false;
        flying = false;
        inPlace.Clear();
		float distance = Random.Range(minDistanceBetween, maxDistanceBetween);
		int count = 0;
		foreach (NonPlayableCharacter user in users)
		{
			user.GetUp();
			int spot = inPlace.Count;
			inPlace.Add(false);
			float angle = ((Mathf.PI * 2f) / users.Count) * count;
			Vector3 pos = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);

			if(user.movement.GoThere(originPosition + pos, true))
			{
				user.movement.onDestinationReached += () =>
				{
					inPlace[spot] = true;
					IsAllInPlace();
				};
			}
			else
			{
				inPlace[spot] = true;
				IsAllInPlace();
			}
			count++;
		}
    }

    void Initialize()
    {
        full = false;
        playing = false;
        carrier = 0;
    }

	public void Throw()
	{
		transform.position += users[carrier].transform.forward * 0.5f;
		Vector3 throwVector = users[carrier].transform.forward;
		rb.AddForce(new Vector3(throwVector.x, 0f, throwVector.z) * horizontalForce * Time.deltaTime);
		rb.AddForce(new Vector3(0f, 1f, 0f) * verticalForce * Time.deltaTime);
		rb.angularVelocity = transform.forward * 100f;
	}

    public override void Update()
    {
        base.Update();
        if(playing)
        {
			if (throwTimer > 0f) throwTimer -= Time.deltaTime;
            else
            {
                if(!flying)
                {
					if (GoodPositions())
					{
						int next = Next();
						if (users.Count > 1)
						{
							LookAtEachOthers();
							users[carrier].transform.forward = -(users[carrier].transform.position - users[next].transform.position).normalized;
						}

						users[carrier].Drop();

						Throw();
						flying = true;

						Game.i.WaitAndDo(1f, () => {
							carrier = next;
							users[carrier].Collect(this);
						});
					}
					else GetInPlace();
				}
                else
                {
					if (users[carrier].IsCarrying(this))
                    {
						if (users.Count > 1) LookAtEachOthers();
						else if (users.Count == 1) users[0].movement.OrientToward(users[0].transform.position + Geometry.GetRandomPointOnCircle(1f));

                        users[carrier].movement.Stop();
                        throwTimer = timeBetweenThrows;
                        flying = false;
                    }
                }
            }
        }
    }

    void LookAtEachOthers()
    {
		List<Vector3> positions = new List<Vector3>();
		foreach (NonPlayableCharacter user in users) positions.Add(user.transform.position);

		Vector3 center = Geometry.CenterOfPoints(positions.ToArray());
		foreach (NonPlayableCharacter user in users)
		{
			user.GetUp();
			user.movement.OrientToward(center);
		}
	}

    void IsAllInPlace()
    {
		foreach(bool b in inPlace) if(!b) return;
		if (!users[carrier].IsCarrying(this)) return;
        playing = true;
        LookAtEachOthers();
    }

    bool IsAllTrue(bool[] array)
    {
        foreach(bool b in array) if(!b) return false;
        return true;
    }

    int Next()
    {
		int newCarrier = carrier + 1;
        if(newCarrier >= users.Count) newCarrier = 0;
		return newCarrier;
    }
}
