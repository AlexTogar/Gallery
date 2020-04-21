using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;


public class ColorCalculate : MonoBehaviour
{

	//Class for describe every node for clustering
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

	//Convert texture into color list
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


	//Calculate the distance between nodes (colors)
	public static float CalcDistance(Color colorOne, Color colorTwo)
	{
		Vector3 a = new Vector3(colorOne.r, colorOne.g, colorOne.b);
		Vector3 b = new Vector3(colorTwo.r, colorTwo.g, colorTwo.b);
		return Vector3.Distance(a, b);
	}

	//Calculate the center of mass of the nodes (colors)
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

	//Class for describe main colors of picture
	public class MainColor
	{
		public Color color { get; set; }
		public int capacity { get; set; }
		public MainColor(Color col = new Color(),int cap = 0)
		{
			color = col;
			capacity = cap;
		}
	}

	//Class for serialize/deserialize main colors list
	[System.Serializable]
	public class SerializableMainColor
	{
		public List<float> color { get; set; }
		public int capacity { get; set; }
		public SerializableMainColor(Color col = new Color(), int cap = 0)
		{
			color = new List<float>();
			color.Add(col.r);
			color.Add(col.g);
			color.Add(col.b);

			capacity = cap;
		}
	}

	//Class to serialize/deserialize main colors list
	[System.Serializable]
	public class SerializableMainColors
	{
		public List<SerializableMainColor> mainColors {get; set;}
		public int hash { get; set; }
		public SerializableMainColors(List<MainColor> mainColorsParam, int hashParam)
		{
			mainColors = new List<SerializableMainColor>()
			{
				new SerializableMainColor(mainColorsParam[0].color, mainColorsParam[0].capacity),
				new SerializableMainColor(mainColorsParam[1].color, mainColorsParam[1].capacity),
				new SerializableMainColor(mainColorsParam[2].color, mainColorsParam[2].capacity)
			};

			hash = hashParam;
		}
	}

	//Make color lighten if it's not enough (using y = a + b(1/x) function)
	public static Color LightUpdate(Color color, bool enabled)
	{
		if (enabled)
		{
			float func(float x)
			{
				return 3.5f*(-0.57f + 1 / (x + 1.45f)) - 0.1f;
			}


			float currentRate = (color.r + color.g + color.b) / (255f * 3);

			float delta = func(currentRate);
			return new Color(color.r + delta, color.g + delta, color.b + delta);
		} else
		{
			return color;
		}

	}

	//Calculate 3 centers (colors) with biggest r,g and b params
	public static List<Color> GetInitialCenters(List<Color> colorList)
	{
		List<Color> centers = new List<Color>();
		centers.Add(new Color());
		centers.Add(new Color());
		centers.Add(new Color());
		Color centerMass = CalcCenterOfMass(colorList);

		try
		{
			centers[0] = centerMass + new Color(0.1f, 0, 0);
			centers[1] = centerMass + new Color(0, 0.1f, 0);
			centers[2] = centerMass + new Color(0, 0, 0.1f);
		} 
		catch 
		{
			centers[0] = Color.red;
			centers[1] = Color.green;
			centers[2] = Color.blue;
		}

			   
		return centers;
	 
	}
	

	//Get main color from texture ordered by lightness
	public static List<MainColor> GetMainColors(Texture2D texture, int n = 12, int valueSamplesOfColor = 2000)
	{
		double compressionCoef = System.Math.Sqrt(texture.height * texture.width / valueSamplesOfColor);
		List<Color> colorList = new List<Color>();
		if (compressionCoef >= 1)
		{
			colorList = TextureToColorsList(texture, (int)compressionCoef);
		} else
		{
			Debug.Log("Error: image too small");
			colorList = TextureToColorsList(texture, 1);
		}

		List<MainColor> centersList = new List<MainColor>();

		//set centers position
		List<Color> initCenters = GetInitialCenters(colorList);
		centersList.Add(new MainColor(initCenters[0], 0));
		centersList.Add(new MainColor(initCenters[1], 0));	
		centersList.Add(new MainColor(initCenters[2], 0));
			   
	
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

				distancesList.Add(new Node(CalcDistance(color, centersList[0].color), 0));
				distancesList.Add(new Node(CalcDistance(color, centersList[1].color), 1));
				distancesList.Add(new Node(CalcDistance(color, centersList[2].color), 2));

				//sort list
				distancesList = distancesList.OrderBy(x => x.distance).ToList();

				int centerNum = distancesList[0].centerNum;
				listCentersPixels[centerNum].Add(color);

			}

			for (int i = 0	; i < 3; i++)
			{
				centersList[i].color = CalcCenterOfMass(listCentersPixels[i]);
				centersList[i].capacity = listCentersPixels[i].Count();
			}
		}

		centersList = UpdateColorListWithNaN(centersList);


		//return List of colors sorted by brightness
		return centersList.OrderBy(x => (x.color.r + x.color.g + x.color.b)).ToList<MainColor>();
	}

	//Replace NaN in main colors list
	public static List<MainColor> UpdateColorListWithNaN(List<MainColor> colorList)
	{
		//calculate valid color
		MainColor mainColor = new MainColor();
		foreach(MainColor color in colorList)
		{
			if (ValidColor(color.color))
			{
				mainColor = color;
			}
		}

		//init result color list
		List<MainColor> resultColorList = new List<MainColor>();
		for (int i = 0; i < colorList.Count(); i++)
		{
			resultColorList.Add(new MainColor());
		}

		//fill reslut color list
		for (int i = 0; i < colorList.Count(); i++)
		{
			if (!ValidColor(colorList[i].color))
			{
				resultColorList[i] = mainColor;
			}
			else
			{
				resultColorList[i] = colorList[i];
			}
		}

		return resultColorList;
	}

	
	//Check the color on NaN value
	public static bool ValidColor(Color color)
	{
		if (float.IsNaN(color.r) || float.IsNaN(color.g) || float.IsNaN(color.b))
		{
			return false;
		} else
		{
			return true;
		}
	}

	//Sort list of main colors by capacity (amount of nodes belonging to every cluster)
	public static List<MainColor> SortByCapacity(List<MainColor> colorList)
	{
		return colorList.OrderBy(x => (x.capacity)).ToList<MainColor>();
	}
}
