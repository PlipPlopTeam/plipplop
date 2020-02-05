using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName="ScriptableObjects/Cloth")]
public class ClothData : ScriptableObject
{
	/*
	[System.Serializable]
	public class TextureModification
	{
		public string propertyName;
		public List<Texture> textures;
	}
	*/
	[System.Serializable]
	public class ColorModification
	{
		public Palette palette;
		public string[] properties;
	}
	
	public string title = "Untitled";
    public Cloth.ESlot slot;
    public GameObject prefab;
	public ColorModification color = new ColorModification();

	public Dictionary<string, Color> GetColors()
	{
		if (color.palette == null || color.properties.Length <= 0)
			return new Dictionary<string, Color>();

		Color[] colors = color.palette.Get(color.properties.Length);
		Dictionary<string, Color> result = new Dictionary<string, Color>();
		for (int i = 0; i < colors.Length; i++) result.Add(color.properties[i], colors[i]);
		return result;
	}

	//public TextureModification[] textures;

	/*
	public Dictionary<string, Color> GetColors()
	{
		Dictionary<string, Color> result = new Dictionary<string, Color>();
		foreach(ColorModification cm in colors)
		{
			result.Add(cm.propertyName, cm.colors.PickRandom());
		}
		return result;
	}

	public Dictionary<string, Texture> GetTextures()
	{
		Dictionary<string, Texture> result = new Dictionary<string, Texture>();
		foreach(TextureModification tm in textures)
		{
			result.Add(tm.propertyName, tm.textures.PickRandom());
		}
		return result;
	}
	*/
}
