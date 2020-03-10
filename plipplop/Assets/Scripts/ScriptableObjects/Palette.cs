using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/Palette")]
public class Palette : ScriptableObject
{
	[System.Serializable]
	public class Element
	{
		public List<Color> list = new List<Color>();
	}

	public List<Element> colors = new List<Element>();

	public Color[] Get(int count)
	{
		List<Color> result = new List<Color>();

		// If colors works in pairs or more
		if(colors.Count > 1)
		{
			int rand = Random.Range(0, colors[0].list.Count);
			for (int i = 0; i < count; i++)
			{
				if (i < colors.Count) result.Add(colors[i].list[rand]);
				else break;
			}
		}
		else if(colors.Count > 0)
		{
			List<Color> pool = new List<Color>(colors[0].list);
			for (int i = 0; i < count; i++)
			{
				if (pool.Count <= 0) break;

				int rand = Random.Range(0, pool.Count);
				result.Add(pool[rand]);
				pool.RemoveAt(rand);
			}
		}

		// Fill the rest with pure WHITE
		if(result.Count < count)
		{
			int dif = count - result.Count;
			for (int i = 0; i < dif; i++) result.Add(Color.red);
		}

		return result.ToArray();
	}
}
