using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SandCastle : Activity
{
	[Header("References")]
	public GameObject castlePrefab;
	[Header("Settings")]
	public float radius = 1f;
	public Vector2 sizeRange;
	public Vector2 offsetRange;
	public Vector2Int towerRange;

	public float timeToComplete = 10f;
	public float refreshTime = 0.5f;

	private List<SkinnedMeshRenderer> towers = new List<SkinnedMeshRenderer>();
	private int selected = 0;
	private float refreshTimer = 0f;
	private bool constructionInProgress = false;
	private bool built = false;

	private void Start()
	{
		Initialize();
	}

	public override void StartUsing(NonPlayableCharacter user)
	{
		base.StartUsing(user);
		user.look.FocusOn(transform);
		user.GoSitThere(transform.position + Geometry.GetRandomPointOnCircle(radius + 1f));
		user.agentMovement.onDestinationReached += () =>
		{
			user.agentMovement.OrientToward(transform.position);
			constructionInProgress = true;
		};
	}

	public override void StopUsing(NonPlayableCharacter user)
	{
		base.StopUsing(user);
		user.look.LooseFocus();
		user.GetUp();
	}

	private void Generate()
	{
		int count = Random.Range(towerRange.x, towerRange.y);
		for (int i = 0; i < count; i++)
		{
			float angle = ((Mathf.PI * 2f) / count) * i;
			Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
			GameObject go = CreateTower();
			go.transform.localPosition = pos + Geometry.GetRandomPointOnCircle(Random.Range(offsetRange.x, offsetRange.y));
			towers.Add(go.GetComponent<SkinnedMeshRenderer>());
		}
	}

	private void Initialize()
	{
		Clear();
		Generate();
		foreach (SkinnedMeshRenderer smr in towers)
			smr.SetBlendShapeWeight(0, 100f);

		selected = 0;
		built = false;
	}

	private void Clear()
	{
		foreach (SkinnedMeshRenderer smr in towers)
			Destroy(smr.gameObject);

		towers.Clear();
	}

	public override void Update()
	{
		base.Update();
		if(constructionInProgress)
		{
			if (refreshTimer > 0) refreshTimer -= Time.deltaTime;
			else
			{
				float blend = towers[selected].GetBlendShapeWeight(0);
				if (blend >= 0f)
				{
					Pyromancer.PlayGameEffect("gfx_sand_poof", towers[selected].transform.position);
					float value = (towers.Count / timeToComplete) * 100f * refreshTime;
					towers[selected].SetBlendShapeWeight(0, blend - value);
				}
				else
				{

					towers[selected].SetBlendShapeWeight(0, 0f);
					selected++;
					if(selected >= towers.Count) Complete();
				}
				refreshTimer = refreshTime;
			}
		}
	}

	public void Stomp()
	{
		foreach (NonPlayableCharacter npc in users)
		{
			npc.emo.Show("Angry", 2f);
		}

		foreach (SkinnedMeshRenderer smr in towers)
		{
			smr.SetBlendShapeWeight(0, 0f);
			smr.SetBlendShapeWeight(1, 100f);
			Pyromancer.PlayGameEffect("gfx_sand_poof", smr.transform.position);
		}
		built = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(built) Stomp();
		else if(constructionInProgress)
		{
			Stomp();
			Initialize();
		}
	}

	public override bool AvailableFor(NonPlayableCharacter npc)
	{
		return base.AvailableFor(npc) && !built;
	}

	void Complete()
	{
		foreach (SkinnedMeshRenderer smr in towers) smr.SetBlendShapeWeight(0, 0f);
		selected = 0;
		refreshTimer = 0f;
		constructionInProgress = false;
		built = true;
	}

	private GameObject CreateTower()
	{
		GameObject go = Instantiate(castlePrefab, transform);
		go.transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
		go.transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
		return go;
	}

#if UNITY_EDITOR
	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Handles.color = new Color32(255, 255, 255, 255);
		Handles.DrawWireDisc(transform.position, Vector3.up, radius);
	}
#endif
}
