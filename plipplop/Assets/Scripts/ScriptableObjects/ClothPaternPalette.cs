using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/ClothPaternPalette")]
public class ClothPaternPalette : ScriptableObject
{
	[System.Serializable]
	public class Patern
	{
		public Texture texture = null;
		public Vector2 tillingRange = new Vector2();
		public int useTextureColor = 0;
	}
	public string texturePropertyName = "Albedo";
	public string tillingPropertyName = "AlbedoTilling";
	public string maskPropertyName = "_UseMask";

	[System.Serializable]
	public class Info
	{
		public Patern patern;
		public string textureProperty = "Albedo";
		public string tillingProperty = "AlbedoTilling";
		public string maskProperty = "_UseMask";

		public Info(Patern p = null, string texture = "Albedo", string tilling = "AlbedoTilling" , string mask = "_UseMask")
		{
			if (p != null) patern = p;
			else patern = new Patern();

			textureProperty = texture;
			tillingProperty = tilling;
			maskProperty = mask;
		}
	}

	public List<Patern> paterns = new List<Patern>();

	public Patern Get()
	{
		if(paterns.Count > 0)
		{
			return paterns.PickRandom();
		}
		else
		{
			Debug.LogWarning("Trying to get Patern from an empty pallet");
			return new Patern();
		}
	}
}
