using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;
public class ColorCalculate : MonoBehaviour
{

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
		for (int i = 0;i < (int)texture.height/compressionCoef; i+=compressionCoef)
		{
			for (int j = 0; j < (int)texture.width/compressionCoef; j+=compressionCoef)
			{
				listPixels.Add(texture.GetPixel(i, j));
			}

		}
		return listPixels;
	}


	public static float CalcDistance(Color colorOne, Color colorTwo)
	{
		Vector3 a = new Vector3(colorOne.a, colorOne.b, colorOne.g);
		Vector3 b = new Vector3(colorTwo.a, colorTwo.b, colorTwo.g);
		return Vector3.Distance(a, b);
	}

	public static Color CalcCenterOfMass(List<Color> colorList)
	{
		Vector3 result = new Vector3(0,0,0);
		foreach(Color node in colorList)
		{
			result += new Vector3(node.r, node.b, node.g);
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
			}

			List<Color> newCentersList = new List<Color>();
			for (int i = 0; i < 3; i++)
			{
				newCentersList.Add(CalcCenterOfMass(listCentersPixels[i]));
			}

			centersList = newCentersList;
		}

		Debug.Log("centers after algorithm processing:" +
			centersList[0].ToString() + "\n" +
			centersList[1].ToString() + "\n" +
			centersList[2].ToString() + "\n");

		return centersList;
	}
}
