using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scrLandscape : MonoBehaviour
{
	public GameObject[] CactusPrefabs;
	public GameObject[] OtherShit;

	void Awake ()
	{
		TerrainData data = GetComponent<Terrain> ().terrainData;
		float[,] heights = data.GetHeights (0, 0, data.heightmapResolution, data.heightmapResolution);
		for (int x = 0; x < heights.GetLength(0); ++x)
			for (int z = 0; z < heights.GetLength(1); ++z)
				heights [x, z] = 0.0f;

		GenerateDunes (ref heights);
		GenerateImpactCrater (ref heights);
		//GenerateSandRipples (ref heights);

		GenerateStuff (heights);

		data.SetHeights (0, 0, heights);
	}

	void Update ()
	{
	
	}

	void GenerateDunes(ref float[,] heights)
	{
		float duneAmplitude = 0.12f;
		float duneFrequency = 0.01f;

		for (int x = 0; x < heights.GetLength(0); ++x)
		{
			for (int z = 0; z < heights.GetLength(1); ++z)
			{
				
				heights[x, z] += duneAmplitude * (Mathf.Max(Mathf.PerlinNoise(x * duneFrequency, z * duneFrequency) -
				                                            0.5f * Mathf.PerlinNoise(10 + x * duneFrequency, 10 + z * duneFrequency), 0)) +
												  duneAmplitude * 0.002f * Mathf.PerlinNoise(x * duneFrequency * 5, z * duneFrequency * 5) + 
												  duneAmplitude * Mathf.Pow (Mathf.PerlinNoise(x * duneFrequency, z * duneFrequency), 3);
				
			}
		}

		// Blur
		int blurAmount = 2;
		for (int x = 0; x < heights.GetLength(0); ++x)
		{
			for (int z = 0; z < heights.GetLength(1); ++z)
			{
				float value = heights[x, z];
				for (int i = 1; i < blurAmount; ++i) 
				{
					value += heights[Mathf.Max (x - i, 0), Mathf.Max (z - i, 0)];
					value += heights[Mathf.Min (x + i, heights.GetLength(0) - 1), Mathf.Min (z + 1, heights.GetLength(1) - 1)];
					value += heights[Mathf.Min (x + i, heights.GetLength(0) - 1), Mathf.Max (z - i, 0)];
					value += heights[Mathf.Max (x - i, 0), Mathf.Min (z + i, heights.GetLength(1) - 1)];
				}
				value /= blurAmount * 4;
				heights[x, z] = value;
			}
		}
	}

	void GenerateImpactCrater(ref float[,] heights)
	{
		float impactRadius = 50.0f;

		for (int x = 0; x < heights.GetLength(0); ++x)
		{
			for (int z = 0; z < heights.GetLength(1); ++z)
			{
				float relX = x - heights.GetLength(0) * 0.5f;
				float relZ = z - heights.GetLength(1) * 0.5f;
				float distance = Mathf.Sqrt(relX * relX + relZ * relZ);
				if (distance < impactRadius)
				{
					heights[x, z] = Mathf.Lerp(0, heights[x, z] * 1.05f, distance / impactRadius);
				}
				else
				{
					if (distance < impactRadius * 1.25f)
					{
						heights[x, z] = Mathf.SmoothStep(heights[x, z] * 1.05f, heights[x, z], (distance - impactRadius) / (impactRadius * 0.25f));
					}
				}
			}
		}
	}

	void GenerateSandRipples(ref float[,] heights)
	{
		float rippleAmplitude = 0.001f;
		float rippleFrequency = 3.0f;

		for (int x = 0; x < heights.GetLength(0); ++x)
		{
			for (int z = 0; z < heights.GetLength(1); ++z)
			{

				heights[x, z] += rippleAmplitude * Mathf.Sin (z * rippleFrequency);

			}
		}
	}

	void GenerateStuff(float[,] heights)
	{
		GameObject[] oases = GameObject.FindGameObjectsWithTag ("Oasis");

		int cacti = 256;

		List<Vector2> positions = new List<Vector2> ();
		for (int i = 0; i < heights.GetLength(0); ++i)
		{
			for (int j = 0; j < heights.GetLength(1); ++j)
			{
				foreach (GameObject oasis in oases)
				{
					if (oasis.GetComponent<BoxCollider>().bounds.Contains(new Vector2(i * 1.9f, j * 1.9f)))
						break;
					else
						positions.Add(new Vector2 (i, j));
				}
			}
		}

		for (int i = 0; i < cacti; ++i)
		{
			int index = Random.Range (i, positions.Count);

			float cactusX = positions[index].x * 1.9f;
			float cactusZ = positions[index].y * 1.9f;
			GameObject cactus = (GameObject)Instantiate(CactusPrefabs[Random.Range (0, CactusPrefabs.Length)], new Vector3(cactusX,  GetComponent<Terrain> ().SampleHeight(new Vector3(cactusX, 0, cactusZ)), cactusZ), Quaternion.Euler(0, Random.Range (0, 360), 0));

			Vector2 temp = positions[index];
			positions[index] = positions[i];
			positions[i] = temp;
		}

		int amountOfShit = 512;
		
		for (int i = 0; i < amountOfShit; ++i)
		{
			int index = Random.Range (i + cacti, positions.Count);
			
			float shitX = positions[index].x * 1.9f;
			float shitZ = positions[index].y * 1.9f;
			GameObject shit = (GameObject)Instantiate(OtherShit[Random.Range (0, OtherShit.Length)], new Vector3(shitX,  GetComponent<Terrain> ().SampleHeight(new Vector3(shitX, 0, shitZ)), shitZ), Quaternion.Euler(0, Random.Range (0, 360), 0));
			shit.transform.localScale = Vector3.one * Random.Range (1.0f, 3.0f);

			Vector2 temp = positions[index];
			positions[index] = positions[i];
			positions[i] = temp;
		}
	}
}