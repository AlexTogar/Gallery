using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;


public class ColorCalculate : MonoBehaviour
{

	public static void SaveTexturesToFile(string fileName, List<Texture2D> textures)
	{
		string json = JsonConvert.SerializeObject(textures);
		System.IO.File.WriteAllText (fileName + ".txt", json);
	}

	public static List<Texture2D> GetTexturesFromFile(string fileName)
	{
		var jsonString = File.ReadAllText(fileName);
		List<Texture2D> textures = JsonConvert.DeserializeObject<List<Texture2D>>(jsonString);
		return textures;
	}

	public static List<Texture2D> GetNotTreatedTextures(string fileName, List<Texture2D> currentTextures)
	{
		List<Texture2D> oldTextures = GetTexturesFromFile(fileName);
		List<Texture2D> resultTextures = new List<Texture2D>();

		foreach (Texture2D currentTexture in currentTextures)
		{
			bool findFlag = false;
			foreach (Texture2D oldTexture in oldTextures)
			{
				if (oldTexture == currentTexture)
				{
					findFlag = true;
					break;
				}
			}

			if (findFlag == false)
			{
				resultTextures.Add(currentTexture);
			}
		}
		return resultTextures;
	}

	public class Node
	{
		public Node(float dis, int num)
		{
			distance = dis;
			centerNum = num;
		}
		public float distance { get; set; }
		public int centerNum { get; set; }
	}

	public static List<Color> TextureToColorsList(Texture2D texture, int compressionCoef)
	{
		List<Color> listPixels = new List<Color>();
		for (int i = 0; i < texture.height; i+=compressionCoef)
		{
			for (int j = 0; j < texture.width; j+=compressionCoef)
			{
				listPixels.Add(texture.GetPixel(i, j));
			}

		}
		return listPixels;
	}


	public static float CalcDistance(Color colorOne, Color colorTwo)
	{
		Vector3 a = new Vector3(colorOne.r, colorOne.g, colorOne.b);
		Vector3 b = new Vector3(colorTwo.r, colorTwo.g, colorTwo.b);
		return Vector3.Distance(a, b);
	}

	public static Color CalcCenterOfMass(List<Color> colorList)
	{
		Vector3 result = new Vector3(0,0,0);
		foreach(Color node in colorList)
		{
			result += new Vector3(node.r, node.g, node.b);
		}
		result = result / colorList.Count();

		return new Color(result.x, result.y, result.z);
	}

	


	public static List<Color> GetMainColors(Texture2D texture, int n = 12)
	{

		List<Color> colorList = TextureToColorsList(texture, 5);
		List<Color> centersList = new List<Color>();

		//set random c+enters position
		for (int i = 0; i < 3; i++)
		{
			centersList.Add(new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f)));
		}
		Debug.Log("centers before algorithm processing:" + 
			centersList[0].ToString() + "\n" + 
			centersList[1].ToString() + "\n" +
			centersList[2].ToString() + "\n");
	
		//calculate lists of pixels belonging to centers
		List<Node> distancesList = new List<Node>();

		for (int j = 0; j < n; j++)
		{
			List<List<Color>> listCentersPixels = new List<List<Color>>();
			for (int i = 0; i < 3; i++)
			{
				listCentersPixels.Add(new List<Color>());
			}

			foreach (Color color in colorList)
			{
				distancesList = new List<Node>();

				distancesList.Add(new Node(CalcDistance(color, centersList[0]), 0));
				distancesList.Add(new Node(CalcDistance(color, centersList[1]), 1));
				distancesList.Add(new Node(CalcDistance(color, centersList[2]), 2));

				//sort list
				distancesList = distancesList.OrderBy(x => x.distance).ToList();

				int centerNum = distancesList[0].centerNum;
				listCentersPixels[centerNum].Add(color);

				float test = CalcDistance(new Color(1, 1, 1), new Color(0,0,0));
			}

			for (int i = 0; i < 3; i++)
			{
				centersList[i] = CalcCenterOfMass(listCentersPixels[i]);
			}
		}

		Debug.Log("centers after algorithm processing:" +
			centersList[0].ToString() + "\n" +
			centersList[1].ToString() + "\n" +
			centersList[2].ToString() + "\n");

		return centersList;
	}
}
