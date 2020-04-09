using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloth : MonoBehaviour
{
    public enum ESlot {TOP_HEAD, HEAD, FACE, NECK, TORSO, LEFT_WRIST, RIGHT_WRIST, RIGHT_HAND, LEFT_HAND, LEGS, RIGHT_FOOT, LEFT_FOOT, BELT};
    public ClothData data;
    public Transform root;
    public List<Transform> attachs = new List<Transform>();
    public Skeleton.Socket socket;
	public Renderer renderer;

    public virtual void Destroy()
    {
        foreach(Transform a in attachs) Destroy(a.gameObject);
        Destroy(root.gameObject);
        if(socket != null) socket.item = null;
        Destroy(this);
    }

	public void SetWeight(float ratio)
	{
		SkinnedMeshRenderer smr = root.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (smr != null)
		{
			if (smr.sharedMesh.blendShapeCount > 0)
			{
				smr.SetBlendShapeWeight(0, ratio);
			}
		}
	}

	public virtual void SetColors(Dictionary<string, Color> colors) // TODO : Should become an array at some point
	{
		Material[] mats = renderer.sharedMaterials;
		for (int i = 0; i < mats.Length; i++)
		{
			foreach (KeyValuePair<string, Color> c in colors)
			{
				mats[i].SetColor(c.Key, c.Value);
			}
		}
		renderer.sharedMaterials = mats;
	}
	public virtual void SetPatern(ClothPaternPalette.Info info) // TODO : Should become an array at some point
	{
		Material[] mats = renderer.sharedMaterials;
		for (int i = 0; i < mats.Length; i++)
		{
			Vector4 vec = new Vector4();
			float tilling = 1f;
			if(info.patern.tillingRange.x == info.patern.tillingRange.y)
			{
				tilling = info.patern.tillingRange.x;
			}
			else
			{
				tilling = Random.Range(info.patern.tillingRange.x, info.patern.tillingRange.y);
			}
			vec.x = vec.y = tilling;

			mats[i].SetTexture(info.textureProperty, info.patern.texture);
			mats[i].SetVector(info.tillingProperty, vec);
			mats[i].SetInt(info.maskProperty, info.patern.useTextureColor);
		}
		renderer.sharedMaterials = mats;
	}

	public virtual void Scale(float amount)
	{
		//foreach (Transform t in attachs) t.localScale = Vector3.one * amount;
		//root.localScale = Vector3.one * amount;
	}

	public virtual void Create(ClothData cloth)
	{
		data = cloth;
		Spawn();
	}
	public virtual void Create(ClothData cloth, Skeleton target)
	{
		data = cloth;
		Spawn();
		Attach(target);
	}

	public virtual void Spawn()
	{
		if (data.slot == ESlot.TORSO || data.slot == ESlot.LEGS || data.slot == ESlot.RIGHT_FOOT || data.slot == ESlot.LEFT_FOOT)
		{
			root = Instantiate(data.prefab, transform).transform;
			foreach (Transform child in root.GetComponentsInChildren<Transform>())
				if (child != root) attachs.Add(child);
		}
		else
		{
			root = Instantiate(data.prefab, transform).transform;
		}

		// COPY MATERIAL
		renderer = root.GetComponentInChildren<Renderer>();
		Material[] mats = renderer.sharedMaterials;
		for (int i = 0; i < mats.Length; i++) mats[i] = Instantiate(mats[i]);
		renderer.sharedMaterials = mats;

		SetColors(data.GetColors());
		SetPatern(data.GetPaternInfo());
	}

	public virtual void Attach(Skeleton target)
	{
		transform.SetParent(target.transform);
		if (data.slot == ESlot.TORSO || data.slot == ESlot.LEGS || data.slot == ESlot.RIGHT_FOOT || data.slot == ESlot.LEFT_FOOT)
			target.Enfold(root);
		else
			target.Attach(root, data.slot);
	}
}

